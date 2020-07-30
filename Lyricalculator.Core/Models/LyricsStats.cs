using System.Collections.Generic;

namespace Lyricalculator.Core.Models
{
    /// <summary>
    /// Represents stats for lyrics for an individual song
    /// </summary>
    /// <remarks>
    /// This same class could be used for an album or a year
    /// </remarks>
    public class LyricsStats
    {
        public int TotalNumberOfWords { get; set; }

        public int NumberOfNonStopWords { get; set; }

        public int NumberOfUniqueNonStopWords { get; set; }

        public string LongestWord { get; set; }

        public Dictionary<string, int> MostPopularNonStopWords { get; set; }

        public Dictionary<int, int> WordLengths { get; set; }
    }
}
