using Lyricalculator.Core.Models;
using System.Threading.Tasks;

namespace Lyricalculator.Core
{
    public interface ILyricsApiQuery
    {
        Task<LyricsResponse> FetchLyrics(Artist artist, Song song);
    }
}
