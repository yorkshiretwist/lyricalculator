using Lyricalculator.Core.Models;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lyricalculator.Core
{
    /// <summary>
    /// Simple service to handle fetching of data from the 3rd party sources
    /// </summary>
    public class MusicService : IMusicService
    {
        private readonly IMusicBrainzQuery _musicBrainzQuery;
        private readonly ILyricsApiQuery _lyricsApiQuery;
        private readonly ILyricsParser _lyricsParser;
        private readonly ICacheManager _cacheManager;

        public MusicService(IMusicBrainzQuery musicBrainzQuery, ILyricsApiQuery lyricsApiQuery, ILyricsParser lyricsParser, ICacheManager cacheManager)
        {
            _musicBrainzQuery = musicBrainzQuery;
            _lyricsApiQuery = lyricsApiQuery;
            _lyricsParser = lyricsParser;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Search artists by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<SearchResponse<Artist>> SearchArtists(string name, int limit = 10)
        {
            var searchResults = await _musicBrainzQuery.FindArtistsAsync(name, simple: true, limit: limit);
            var response = new SearchResponse<Artist>
            {
                TotalCount = searchResults.TotalResults,
                Items = searchResults.Results?.Select(x => Artist.FromMusicBrainzModel(x.Item))
            };
            return response;
        }

        /// <summary>
        /// Get the lyrics and stats for a single song
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="song"></param>
        /// <returns></returns>
        public async Task<LyricsResponse> GetLyrics(Artist artist, Song song)
        {
            var cachedSong = _cacheManager.GetSong(song.Id);
            if (cachedSong != null && !string.IsNullOrWhiteSpace(cachedSong.Lyrics))
            {
                return new LyricsResponse
                {
                    Status = cachedSong.LyricsStatus,
                    Lyrics = cachedSong.Lyrics
                };
            }

            var lyricsResponse = await _lyricsApiQuery.FetchLyrics(artist, song);

            song.Lyrics = lyricsResponse.Lyrics;
            song.LyricsStatus = lyricsResponse.Status;

            if (song.LyricsStatus == LyricsStatus.Found)
            {
                song.LyricsStats = _lyricsParser.Parse(song.Lyrics);
            }

            // cache the song for performance, even if it hasn't been found
            // manually delete the cache if you want to try to get the lyrics again
            _cacheManager.StoreSong(song);

            return lyricsResponse;
        }

        /// <summary>
        /// Get albums for an artist
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Album>> GetAlbums(Artist artist)
        {
            var cachedAlbums = _cacheManager.GetArtistAlbums(artist.Id);
            if (cachedAlbums != null)
            {
                return cachedAlbums;
            }

            // it would be better to get this from the cache, but that would require more type mapping than I want to do at the moment
            var musicBrainzArtist = await LookupArtist(artist.Id);

            var albums = new List<Album>();

            var skip = 0;

            while (true)
            {
                var releases = await _musicBrainzQuery.BrowseReleaseGroupsAsync(musicBrainzArtist, 100, skip, Include.ReleaseRelationships, ReleaseType.Album);

                if (releases == null || releases.Results == null || !releases.Results.Any())
                {
                    break;
                }

                albums.AddRange(releases.Results.Where(r => !r.SecondaryTypeIds.Any()).Select(r => Album.FromMusicBrainzModel(r)));

                skip += 100;
            }

            // now we've got all albums, get the first release for each, so we can get the songs
            foreach(var album in albums)
            {
                var releaseGroup = await _musicBrainzQuery.LookupReleaseGroupAsync(album.ReleaseGroupId, Include.Releases, ReleaseStatus.Official);
                if (releaseGroup == null || releaseGroup.Releases == null || !releaseGroup.Releases.Any())
                {
                    continue;
                }

                // try to get the first release of the album
                // this will fail if the first release is not in the max 25 releases returned as part of the LookupReleaseGroupAsync call
                var firstRelease = releaseGroup.Releases.OrderBy(r => r.Date?.NearestDate).FirstOrDefault();
                if (firstRelease == null)
                {
                    continue;
                }
                album.ReleaseId = firstRelease.Id;

                // now get the songs
                // TODO: this will only get the first 25 songs
                var release = await _musicBrainzQuery.LookupReleaseAsync(album.ReleaseId, Include.Recordings);
                var firstMedium = release.Media?.FirstOrDefault();
                if (firstMedium == null)
                {
                    continue;
                }

                album.Songs = firstMedium.Tracks?.Select(t => Song.FromMusicBrainzModel(t));
            }

            _cacheManager.StoreArtistAlbums(artist.Id, albums);

            return albums;
        }

        /// <summary>
        /// Calculate the lyric stats for an artist
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public async Task<ArtistLyricStats> CalculateStats(Artist artist)
        {
            var stats = new ArtistLyricStats();

            // get the list of songs for the artist; this does not include the song stats
            var albums = await GetAlbums(artist);
            stats.Albums = albums;

            var allSongs = albums.Where(a => a.Songs != null && a.Songs.Any()).SelectMany(a => a.Songs);
            stats.Songs = allSongs.Count();

            // load each song, with its stats, so we can aggregate the numbers
            var songsWithStats = allSongs.Select(s => _cacheManager.GetSong(s.Id));

            stats.SongsWithLyrics = songsWithStats.Count(s => s.LyricsStats.TotalNumberOfWords > 0);

            stats.TotalDistinctNonStopWords = songsWithStats.Sum(s => s.LyricsStats.NumberOfUniqueNonStopWords);

            stats.LongestWords = songsWithStats
                .Where(s => !string.IsNullOrWhiteSpace(s.LyricsStats.LongestWord))
                .Select(s => s.LyricsStats.LongestWord)
                .OrderByDescending(w => w.Length);

            // this gnarly nested loop could be made better with some clever lambdas no doubt
            stats.MostPopularNonStopWords = new Dictionary<string, int>();
            var songPopularWords = songsWithStats.Where(s => s.LyricsStats.MostPopularNonStopWords != null && s.LyricsStats.MostPopularNonStopWords.Any()).Select(s => s.LyricsStats.MostPopularNonStopWords);
            foreach(var thisSongPopularWords in songPopularWords)
            {
                foreach(var kvp in thisSongPopularWords)
                {
                    if (stats.MostPopularNonStopWords.ContainsKey(kvp.Key))
                    {
                        stats.MostPopularNonStopWords[kvp.Key] = stats.MostPopularNonStopWords[kvp.Key] + kvp.Value;
                    } else
                    {
                        stats.MostPopularNonStopWords.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            stats.AverageNumberOfWords = songsWithStats.Select(s => s.LyricsStats.NumberOfNonStopWords).Average();

            stats.MaximumNumberOfNonStopWords = songsWithStats.Max(s => s.LyricsStats.NumberOfNonStopWords);
            stats.MinimumNumberOfNonStopWords = songsWithStats.Where(s => s.LyricsStats.NumberOfNonStopWords > 0).Min(s => s.LyricsStats.NumberOfNonStopWords);

            /*
             * I've run out of time, but with a bit more work we could get things like:
             * - Stats for each album
             * - Stats for each album year
             */

            return stats;
        }

        private async Task<IArtist> LookupArtist(Guid artistId)
        {
            return await _musicBrainzQuery.LookupArtistAsync(artistId);
        }
    }
}
