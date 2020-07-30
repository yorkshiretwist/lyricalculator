using Lyricalculator.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lyricalculator.Core
{
    public interface IMusicService
    {
        Task<SearchResponse<Artist>> SearchArtists(string name, int limit = 10);

        Task<IEnumerable<Album>> GetAlbums(Artist artist);

        Task<LyricsResponse> GetLyrics(Artist artist, Song song);

        Task<ArtistLyricStats> CalculateStats(Artist artist);
    }
}
