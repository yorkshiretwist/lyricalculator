using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Browses;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using System;
using System.Threading.Tasks;

namespace Lyricalculator.Core
{
    /// <summary>
    /// Simple wrapper for the MusicBrainz Query class
    /// </summary>
    public class MusicBrainzQuery : IMusicBrainzQuery
    {
        private readonly Query _query;

        public MusicBrainzQuery(MusicBrainzSettings musicBrainzSettings)
        {
            _query = new Query(musicBrainzSettings.AppName, musicBrainzSettings.AppVersion, musicBrainzSettings.AppContact);
        }

        public async Task<IBrowseResults<IReleaseGroup>> BrowseReleaseGroupsAsync(IArtist artist, int? limit = null, int? offset = null, Include inc = Include.None, ReleaseType? type = null)
        {
            return await _query.BrowseReleaseGroupsAsync(artist, limit, offset, inc, type);
        }

        public async Task<ISearchResults<ISearchResult<IArtist>>> FindArtistsAsync(string query, int? limit = null, int? offset = null, bool simple = false)
        {
            return await _query.FindArtistsAsync(query, limit, offset, simple);
        }

        public async Task<IArtist> LookupArtistAsync(Guid mbid, Include inc = Include.None, ReleaseType? type = null, ReleaseStatus? status = null)
        {
            return await _query.LookupArtistAsync(mbid, inc, type, status);
        }

        public async Task<IRelease> LookupReleaseAsync(Guid mbid, Include inc = Include.None)
        {
            return await _query.LookupReleaseAsync(mbid, inc);
        }

        public async Task<IReleaseGroup> LookupReleaseGroupAsync(Guid mbid, Include inc = Include.None, ReleaseStatus? status = null)
        {
            return await _query.LookupReleaseGroupAsync(mbid, inc, status);
        }
    }
}
