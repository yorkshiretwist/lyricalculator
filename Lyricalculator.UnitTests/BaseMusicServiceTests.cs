using Lyricalculator.Core;
using Lyricalculator.UnitTests.Models;
using Moq;
using System;

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
    }
}
