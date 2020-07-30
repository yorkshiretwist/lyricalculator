using Lyricalculator.Core.Models;

namespace Lyricalculator.Core
{
    public interface ILyricsParser
    {
        LyricsStats Parse(string lyrics);
    }
}
