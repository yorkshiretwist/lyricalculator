using FluentAssertions;
using Lyricalculator.Core;
using Lyricalculator.Core.Models;
using Lyricalculator.UnitTests.Models;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Browses;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyricalculator.UnitTests
{
    [TestFixture]
    public class MusicServiceGetAlbumsTests : BaseMusicServiceTests
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
        public void GetLyrics_NullAlbumsReturnedFromApi_ReturnsCorrectResponse()
        {
            var musicBrainzQueryMock = new Mock<IMusicBrainzQuery>();
            musicBrainzQueryMock.Setup(x => x.LookupArtistAsync(It.IsAny<Guid>(), It.IsAny<Include>(), It.IsAny<ReleaseType>(), It.IsAny<ReleaseStatus>())).ReturnsAsync(GetArtist("Bob Dylan"));
            musicBrainzQueryMock.Setup(x => x.BrowseReleaseGroupsAsync(It.IsAny<IArtist>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Include>(), It.IsAny<ReleaseType>())).ReturnsAsync((IBrowseResults<IReleaseGroup>)null);

            var response = GetService(musicBrainzQueryMock: musicBrainzQueryMock).GetAlbums(new Artist()).Result;

            response.Should().NotBeNull();
            response.Should().BeEmpty();
            musicBrainzQueryMock.Verify(x => x.LookupReleaseGroupAsync(It.IsAny<Guid>(), It.IsAny<Include>(), It.IsAny<ReleaseStatus>()), Times.Never);
            musicBrainzQueryMock.Verify(x => x.LookupReleaseAsync(It.IsAny<Guid>(), It.IsAny<Include>()), Times.Never);
        }

        [Test]
        public void GetLyrics_AlbumsCached_ReturnsCorrectResponse()
        {
            var cachedAlbums = GetAlbums();

            var cacheManagerMock = new Mock<ICacheManager>();
            cacheManagerMock.Setup(x => x.GetArtistAlbums(It.IsAny<Guid>())).Returns(cachedAlbums);

            var musicBrainzQueryMock = new Mock<IMusicBrainzQuery>();

            var response = GetService(musicBrainzQueryMock: musicBrainzQueryMock, cacheManagerMock: cacheManagerMock).GetAlbums(new Artist()).Result;

            response.Should().NotBeNull();
            response.Count().Should().Equals(cachedAlbums.Count());
            foreach(var album in cachedAlbums)
            {
                response.Any(a => a.ReleaseGroupId == album.ReleaseGroupId && a.Title == album.Title).Should().BeTrue();
            }

            musicBrainzQueryMock.Verify(x => x.LookupArtistAsync(It.IsAny<Guid>(), It.IsAny<Include>(), It.IsAny<ReleaseType>(), It.IsAny<ReleaseStatus>()), Times.Never);
            musicBrainzQueryMock.Verify(x => x.BrowseReleaseGroupsAsync(It.IsAny<IArtist>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Include>(), It.IsAny<ReleaseType>()), Times.Never);
            musicBrainzQueryMock.Verify(x => x.LookupReleaseGroupAsync(It.IsAny<Guid>(), It.IsAny<Include>(), It.IsAny<ReleaseStatus>()), Times.Never);
            musicBrainzQueryMock.Verify(x => x.LookupReleaseAsync(It.IsAny<Guid>(), It.IsAny<Include>()), Times.Never);
        }
    }
}
