using System.Collections.Generic;

namespace Lyricalculator.Core.Models
{
    /// <summary>
    /// Represents lyric stats for all songs for an artist
    /// </summary>
    public class ArtistLyricStats
    {
        public IEnumerable<Album> Albums { get;  set; }

        public int Songs { get; set; }

        public int SongsWithLyrics { get; set; }

        public Dictionary<string, LyricsStats> SongLyricsStats { get; set; }

        public double AverageNumberOfWords { get; set; }

        public IEnumerable<string> LongestWords { get; set; }

        public int TotalDistinctNonStopWords { get; set; }

        public int MaximumNumberOfNonStopWords { get; set; }

        public int MinimumNumberOfNonStopWords { get; set; }

        public Dictionary<string, int> MostPopularNonStopWords { get; set; }
    }
}
