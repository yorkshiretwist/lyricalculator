using FluentAssertions;
using Lyricalculator.Core;
using Lyricalculator.Core.Models;
using Lyricalculator.UnitTests.Models;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Lyricalculator.UnitTests
{
    [TestFixture]
    public class MusicServiceGetLyricsTests : BaseMusicServiceTests
    {

        private IReadOnlyList<ISearchResult<IArtist>> GetArtistSearchResults(List<string> names)
        {
            var list = new List<ISearchResult<IArtist>>();
            foreach(var name in names)
            {
                var artist = GetArtist(name);
                list.Add(new MockSearchResult<IArtist>(artist));
            }
            return list;
        }

        private LyricsResponse GetMockFetchLyricsResponse(LyricsStatus status, string lyrics)
        {
            return new LyricsResponse
            {
                Lyrics = lyrics,
                Status = status
            };
        }

        private void VerifyGetLyricsResponse(LyricsResponse response, LyricsStatus expectedStatus, string expectedLyrics, Mock<ILyricsApiQuery> lyricsApiQueryMock)
        {
            response.Should().NotBeNull();
            response.Status.Should().Equals(expectedStatus);
            response.Lyrics.Should().Equals(expectedLyrics);

            lyricsApiQueryMock.Verify(x => x.FetchLyrics(It.IsAny<Artist>(), It.IsAny<Song>()), Times.Once);
        }

        [Test]
        public void GetLyrics_LyricsNotFound_ReturnsCorrectResponse()
        {
            var apiResponse = GetMockFetchLyricsResponse(LyricsStatus.NotFound, null);

            var lyricsApiQueryMock = new Mock<ILyricsApiQuery>();
            lyricsApiQueryMock.Setup(x => x.FetchLyrics(It.IsAny<Artist>(), It.IsAny<Song>())).ReturnsAsync(apiResponse);

            var response = GetService(lyricsApiQueryMock: lyricsApiQueryMock).GetLyrics(new Artist(), new Song()).Result;

            VerifyGetLyricsResponse(response, apiResponse.Status, apiResponse.Lyrics, lyricsApiQueryMock);
        }

        [Test]
        public void GetLyrics_LyricsFound_ReturnsCorrectResponse()
        {
            var apiResponse = GetMockFetchLyricsResponse(LyricsStatus.Found, "Here are the lyrics");

            var lyricsApiQueryMock = new Mock<ILyricsApiQuery>();
            lyricsApiQueryMock.Setup(x => x.FetchLyrics(It.IsAny<Artist>(), It.IsAny<Song>())).ReturnsAsync(apiResponse);

            var response = GetService(lyricsApiQueryMock: lyricsApiQueryMock).GetLyrics(new Artist(), new Song()).Result;

            VerifyGetLyricsResponse(response, apiResponse.Status, apiResponse.Lyrics, lyricsApiQueryMock);
        }

        [Test]
        public void GetLyrics_LyricsCached_ReturnsCorrectResponse()
        {
            var cachedSong = new Song
            {
                LyricsStatus = LyricsStatus.Found,
                Lyrics = "Here are the lyrics"
            };

            var cacheManagerMock = new Mock<ICacheManager>();
            cacheManagerMock.Setup(x => x.GetSong(It.IsAny<Guid>())).Returns(cachedSong);

            var lyricsApiQueryMock = new Mock<ILyricsApiQuery>();            

            var response = GetService(lyricsApiQueryMock: lyricsApiQueryMock, cacheManagerMock: cacheManagerMock).GetLyrics(new Artist(), new Song()).Result;

            response.Should().NotBeNull();
            response.Status.Should().Equals(cachedSong.LyricsStatus);
            response.Lyrics.Should().Equals(cachedSong.Lyrics);

            lyricsApiQueryMock.Verify(x => x.FetchLyrics(It.IsAny<Artist>(), It.IsAny<Song>()), Times.Never);
        }
    }
}
