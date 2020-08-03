using Lyricalculator.Core;
using Lyricalculator.Core.Models;
using Lyricalculator.UnitTests.Models;
using Moq;
using System;
using System.Collections.Generic;

namespace Lyricalculator.UnitTests
{
    public class BaseMusicServiceTests
    {
        protected MusicService GetService(Mock<IMusicBrainzQuery> musicBrainzQueryMock = null, Mock<ILyricsApiQuery> lyricsApiQueryMock = null, Mock<ILyricsParser> lyricsParserMock = null, Mock<ICacheManager> cacheManagerMock = null)
        {
            if (musicBrainzQueryMock == null)
            {
                musicBrainzQueryMock = new Mock<IMusicBrainzQuery>();
            }
            if (lyricsApiQueryMock == null)
            {
                lyricsApiQueryMock = new Mock<ILyricsApiQuery>();
            }
            if (lyricsParserMock == null)
            {
                lyricsParserMock = new Mock<ILyricsParser>();
            }
            if (cacheManagerMock == null)
            {
                cacheManagerMock = new Mock<ICacheManager>();
            }
            return new MusicService(musicBrainzQueryMock.Object, lyricsApiQueryMock.Object, lyricsParserMock.Object, cacheManagerMock.Object);
        }

        protected MockArtist GetArtist(string name)
        {
            return new MockArtist(Guid.NewGuid(), name, "GB", new MockLifeSpan(new DateTime(1950, 1, 1)));
        }

        protected IEnumerable<Album> GetAlbums()
        {
            return new List<Album>
            {
                new Album
                {
                    Title = "The Fresh And Exciting Second Album",
                    ReleaseGroupId = Guid.NewGuid(),
                    ReleaseId = Guid.NewGuid(),
                    Year = 2018,
                    Songs = GetAlbumSongs(9)
                },
                new Album
                {
                    Title = "The Difficult Second Album",
                    ReleaseGroupId = Guid.NewGuid(),
                    ReleaseId = Guid.NewGuid(),
                    Year = 2020,
                    Songs = GetAlbumSongs(12)
                }
            };
        }

        protected List<Song> GetAlbumSongs(int numSongs)
        {
            var list = new List<Song>();
            for(var x = 1; x <= numSongs; x++)
            {
                list.Add(new Song
                {
                    Id = Guid.NewGuid(),
                    Title = $"Song {x}",
                    Position = x,
                    Length = new TimeSpan(0, Rando(1, 5), Rando(30, 59)),
                    LyricsStatus = LyricsStatus.Found,
                    Lyrics = $"The lyrics for song {x}",
                    LyricsStats = new LyricsStats
                    {
                        LongestWord = "honalulu",
                        MostPopularNonStopWords = new Dictionary<string, int>
                        {
                            { "borked", Rando(5, 7) },
                            { "grok", Rando(4, 5) },
                            { "woo", 1 },
                            { "yay", 1 },
                            { "hoopla", 1 },
                        },
                        NumberOfNonStopWords = Rando(123, 212),
                        NumberOfUniqueNonStopWords = Rando(67, 121),
                        TotalNumberOfWords = Rando(212, 361),
                        WordLengths = new Dictionary<int, int>
                        {
                            { 2, Rando(5, 7) },
                            { 3, Rando(4, 7) },
                            { 4, Rando(3, 6) },
                            { 5, Rando(3, 5) },
                            { 6, Rando(2, 3) },
                            { 7, Rando(2, 3) },
                            { 8, Rando(1, 2) },
                            { 9, Rando(1, 2) }
                        }
                    }
                });
            }
            return list;
        }

        private int Rando(int min, int max)
        {
            return new Random().Next(min, max);
        }
    }
}
