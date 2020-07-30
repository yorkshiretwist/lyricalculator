using System.Collections.Generic;

namespace Lyricalculator.Core
{
    public class LyricsParserSettings
    {
        public int MinimumWordLength { get; set; }

        public char[] Punctuation { get; set; }

        public IEnumerable<string> StopWords { get; set; }
    }
}
