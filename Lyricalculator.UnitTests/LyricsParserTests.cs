using FluentAssertions;
using Lyricalculator.Core;
using Lyricalculator.Core.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Lyricalculator.UnitTests
{
    [TestFixture]
    public class LyricsParserTests
    {
        private LyricsParser GetParser()
        {
            var settings = new LyricsParserSettings
            {
                MinimumWordLength = 2,
                StopWords = new List<string> { "and", "the" },
                Punctuation = new List<char> { ',', '.', '!' }.ToArray()
            };

            return new LyricsParser(settings);
        }

        private string GetLyrics()
        {
            return "Here are my lyrics,\r\nand I know the tune as well! Lyrics!";
        }

        [Test]
        public void Parse_GivenNullLyrics_ReturnsDefaultStats()
        {
            var response = GetParser().Parse(null);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(new LyricsStats());
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\n")]
        [TestCase("\t")]
        public void Parse_GivenEmptyLyrics_ReturnsDefaultStats(string lyrics)
        {
            var response = GetParser().Parse(lyrics);

            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(new LyricsStats());
        }

        [Test]
        public void Parse_GivenLyrics_ReturnsCorrectStats()
        {
            var response = GetParser().Parse(GetLyrics());
            
            response.Should().NotBeNull();
            response.TotalNumberOfWords.Should().Equals(11);
            response.NumberOfNonStopWords.Should().Equals(8);
            response.NumberOfUniqueNonStopWords.Should().Equals(7);
            response.LongestWord.Should().Equals("lyrics");

            var wordLengths = response.WordLengths.OrderByDescending(w => w.Key);
            wordLengths.First().Key.Should().Equals(6);
            wordLengths.First().Value.Should().Equals(2);

            var popularWords = response.MostPopularNonStopWords.OrderByDescending(w => w.Value);
            popularWords.First().Key.Should().Be("lyrics");
            popularWords.First().Value.Should().Be(2);
        }
    }
}
