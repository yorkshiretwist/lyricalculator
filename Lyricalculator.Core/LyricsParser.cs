using Lyricalculator.Core.Models;
using System;
using System.Linq;

namespace Lyricalculator.Core
{
    public class LyricsParser : ILyricsParser
    {
        private readonly LyricsParserSettings _settings;

        public LyricsParser(LyricsParserSettings settings)
        {
            _settings = settings;
        }

        public LyricsStats Parse(string lyrics)
        {
            var stats = new LyricsStats();

            // remove common punctuation
            string[] temp = lyrics.Split(_settings.Punctuation, StringSplitOptions.RemoveEmptyEntries);
            lyrics = string.Join(" ", temp);

            // flatten lines and get words
            var words = lyrics
                .Replace('\r', ' ')
                .Replace('\n', ' ')
                .Split(' ');

            // remove invalid words
            var validWords = words.Where(w => !string.IsNullOrWhiteSpace(w) && w.Length >= _settings.MinimumWordLength);

            stats.TotalNumberOfWords = validWords.Count();

            var nonStopWords = validWords.Where(w => !_settings.StopWords.Contains(w, StringComparer.OrdinalIgnoreCase));
            stats.NumberOfNonStopWords = nonStopWords.Count();
            stats.NumberOfUniqueNonStopWords = nonStopWords.Distinct().Count();

            stats.LongestWord = nonStopWords.OrderByDescending(w => w.Length).First();

            stats.MostPopularNonStopWords = nonStopWords
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.First(), g => g.Count());

            stats.WordLengths = nonStopWords
                .GroupBy(w => w.Length)
                .OrderBy(g => g.First().Length)
                .ToDictionary(g => g.First().Length, g => g.Count());

            return stats;
        }
    }
}
