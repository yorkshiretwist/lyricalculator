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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyricalculator.UnitTests
{
    [TestFixture]
    public class MusicServiceFindArtistTests : BaseMusicServiceTests
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

        private Mock<ISearchResults<ISearchResult<IArtist>>> GetMockFindArtistsResponse(List<string> names = null)
        {
            var queryResponse = new Mock<ISearchResults<ISearchResult<IArtist>>>();
            queryResponse.SetupGet(x => x.TotalResults).Returns((names == null || !names.Any()) ? 0 : names.Count);
            if (names == null || !names.Any())
            {
                queryResponse.SetupGet(x => x.Results).Returns((IReadOnlyList<ISearchResult<IArtist>>)null);
            } else
            {
                queryResponse.SetupGet(x => x.Results).Returns(GetArtistSearchResults(names));
            }
            return queryResponse;
        }

        private void VerifyFindArtistsResponse(SearchResponse<Artist> response, Mock<IMusicBrainzQuery> musicBrainzQueryMock, List<string> names = null)
        {
            var expectedArtistCount = (names == null || !names.Any()) ? 0 : names.Count;

            response.Should().NotBeNull();
            response.TotalCount.Should().Equals(expectedArtistCount);

            if (expectedArtistCount == 0)
            {
                response.Items.Should().BeNull();
            } else
            {
                for(var x = 0; x < expectedArtistCount; x++)
                {
                    response.Items.ElementAt(x).Name.Should().Equals(names.ElementAt(x));
                }
            }

            musicBrainzQueryMock.Verify(x => x.FindArtistsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public void SearchArtists_NoArtistsFound_ReturnsCorrectResponse()
        {
            var queryResponse = GetMockFindArtistsResponse();

            var musicBrainzQueryMock = new Mock<IMusicBrainzQuery>();
            musicBrainzQueryMock.Setup(x => x.FindArtistsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool>())).ReturnsAsync(queryResponse.Object);

            var response = GetService(musicBrainzQueryMock).SearchArtists("test").Result;

            VerifyFindArtistsResponse(response, musicBrainzQueryMock);
        }

        [Test]
        public void SearchArtists_OneArtistFound_ReturnsCorrectResponse()
        {
            var artistNames = new List<string> { "David Bowie" };
            var queryResponse = GetMockFindArtistsResponse(artistNames);      
            
            var musicBrainzQueryMock = new Mock<IMusicBrainzQuery>();
            musicBrainzQueryMock.Setup(x => x.FindArtistsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool>())).ReturnsAsync(queryResponse.Object);

            var response = GetService(musicBrainzQueryMock).SearchArtists("test").Result;

            VerifyFindArtistsResponse(response, musicBrainzQueryMock, artistNames);
        }

        [Test]
        public void SearchArtists_TwoArtistsFound_ReturnsCorrectResponse()
        {
            var artistNames = new List<string> { "David Bowie", "Dave Bainbridge" };
            var queryResponse = GetMockFindArtistsResponse(artistNames);

            var musicBrainzQueryMock = new Mock<IMusicBrainzQuery>();
            musicBrainzQueryMock.Setup(x => x.FindArtistsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<bool>())).ReturnsAsync(queryResponse.Object);

            var response = GetService(musicBrainzQueryMock).SearchArtists("test").Result;

            VerifyFindArtistsResponse(response, musicBrainzQueryMock, artistNames);
        }
    }
}
