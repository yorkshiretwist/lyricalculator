using Lyricalculator.Core;
using Lyricalculator.Core.Models;
using Lyricalculator.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Lyricalculator.CLI
{
    class Program
    {
        private static MusicBrainzSettings _musicBrainzSettings;
        private static LyricsApiSettings _lyricsApiSettings;
        private static LyricsParserSettings _lyricsParserSettings;
        private static IMusicService _musicService;
        private static Spinner _spinner;

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _spinner = new Spinner();

            var lyricsParser = new LyricsParser(_lyricsParserSettings);
            var cacheManager = new CacheManager(Path.Combine(Path.GetTempPath(), "Lyricalculator"));

            _musicService = new MusicService(_musicBrainzSettings, _lyricsApiSettings, lyricsParser, cacheManager);

            WriteLogo();
            Start();
            End();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            _musicBrainzSettings = new MusicBrainzSettings();
            configuration.GetSection("MusicBrainz").Bind(_musicBrainzSettings);

            _lyricsApiSettings = new LyricsApiSettings();
            configuration.GetSection("LyricsApi").Bind(_lyricsApiSettings);

            _lyricsParserSettings = new LyricsParserSettings();
            configuration.GetSection("LyricsParser").Bind(_lyricsParserSettings);
        }

        private static void Start()
        {
            var artistName = GetString("Enter the name of an artist, then press 'enter':");
            Write("Thanks, I'll try to find that artist...", true);

            _spinner.Start();
            var artists = _musicService.SearchArtists(artistName).Result;
            _spinner.Stop();

            if (artists.TotalCount == 0)
            {
                Error("Oh no, there were no artists found for your search! Try again.", true);
                Start();
            }

            if (artists.TotalCount == 1)
            {
                LoadArtist(artists.Items.First());
                return;
            }

            Success($"Good news! We found {artists.TotalCount} artists for your search.{(artists.TotalCount > 10 ? " Here are the first 10 of them" : string.Empty)}", true);
            DisplayArtistList(artists.Items);

            var artistNumber = GetNumber("Please enter the number of the artist you want to load", 1, artists.Items.Count());
            LoadArtist(artists.Items.ElementAt(artistNumber - 1));
        }

        private static void DisplayArtistList(IEnumerable<Artist> artists)
        {
            var counter = 1;
            foreach (var artist in artists)
            {
                var artistLine = $"{counter}: {artist.Name}";
                if (!string.IsNullOrWhiteSpace(artist.Country) || artist.Lifespan?.BeginYear != null)
                {
                    artistLine += " (";
                    if (!string.IsNullOrWhiteSpace(artist.Country))
                    {
                        artistLine += artist.Country;
                        if (artist.Lifespan?.BeginYear != null)
                        {
                            artistLine += ", ";
                        }
                    }
                    if (artist.Lifespan?.BeginYear != null)
                    {
                        artistLine += $"{artist.Lifespan.BeginYear} - {artist.Lifespan?.EndYear}";
                    }
                    artistLine += ")";
                }
                Write($"{artistLine}");
                counter++;
            }
            Write("");
        }

        private static void End()
        {
            Write("");
            Write("Thank you for playing LyriCalculator! Have a nice day.");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private static void LoadArtist(Artist artist)
        {
            Write("Thanks, we're loading the songs for this artist:", true);
            Write(artist.Name);
            Write($"Country: {artist.Country}");
            Write($"Years: {artist.Lifespan?.BeginYear} - {artist.Lifespan?.EndYear}", true);

            Write("This might take a while, please be patient...");
            _spinner.Start();
            var albums = _musicService.GetAlbums(artist).Result;
            _spinner.Stop();

            var allSongs = albums.Where(a => a.Songs != null && a.Songs.Any()).SelectMany(a => a.Songs);

            Success($"We found {albums.Count()} albums containing {allSongs.Count()} songs. Want to see the list?");
            var showSongList = GetString("Type 'y' for yes, 'n' for no", new List<string> { "y", "n" });
            if (showSongList == "y")
            {
                DisplayAlbums(albums);
            }

            Write("");
            Write("OK, are you ready to look up the lyrics for this artist? This can take a LONG time, so go make a cuppa when you've started this. Press 'enter' to start:");
            Console.ReadLine();
            FetchLyrics(artist, allSongs);

            Write("");
            Write("We've fetched all the lyrics we can, we can now generate stats. Press 'enter' to start calculation:");
            Console.ReadLine();
            CalculateStats(artist);
        }

        private static void CalculateStats(Artist artist)
        {
            var stats = _musicService.CalculateStats(artist).Result;
            Write("");
            Write($"OK, here are the lyric stats for {artist.Name}:");
            Write("");
            Write($"In {stats.Albums.Count()} album{stats.Albums.Count().Pluralise()} we found {stats.Songs} song{stats.Songs.Pluralise()}.");
            Write("");
            Write($"Of these, we could get the lyrics for {stats.SongsWithLyrics} song{stats.SongsWithLyrics.Pluralise()}, with a total of {stats.TotalDistinctNonStopWords} unique words being used (after removing stop words).");
            Write("");
            Success($"So that's an average of {Math.Round(stats.AverageNumberOfWords, 2)} words per song.");
            Write("");
            Write("But wait, there's more:");
            Write("");
            Write($"The biggest number of words in a song (after removing stop words) was {stats.MaximumNumberOfNonStopWords}");
            Write("");
            Write($"And the smallest number of words in a song (after removing stop words) was {stats.MinimumNumberOfNonStopWords}");
            Write("");
            Write("Here are the 10 longest words we found:");
            foreach(var longWord in stats.LongestWords.OrderByDescending(x => x.Length).Take(10))
            {
                Write($"  {longWord}");
            }
            Write("");
            Write("And here are the 10 most popular words:");
            foreach (var kvp in stats.MostPopularNonStopWords.OrderByDescending(x => x.Value).Take(10))
            {
                Write($"  {kvp.Key} ({kvp.Value} times)");
            }
            Write("");
            Write("That was fun!");

        }

        private static void FetchLyrics(Artist artist, IEnumerable<Song> songs)
        {
            Write("");
            Write($"Cool, we're fetching the lyrics for {songs.Count()} songs...");
            Write("");

            // this would be faster multi-threaded, but I don't want to hammer api.lyrics.ovh 
            foreach (var song in songs)
            {
                Write($"  '{song.Title}'...");
                var lyricResponse = _musicService.GetLyrics(artist, song).Result;
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                if (lyricResponse.Status == LyricsStatus.Found)
                {
                    Success($"  Lyrics fetched for '{song.Title}'");
                } else
                {
                    Error($"  Lyrics could not be fetched for '{song.Title}'");
                }
            }
        }

        private static void DisplayAlbums(IEnumerable<Album> albums)
        {
            foreach (var album in albums.OrderBy(a => a.Year))
            {
                Write("");
                Write($"  {album.Title}{(album.Year.HasValue ? " (" + album.Year.Value + ")" : string.Empty)}");
                if (album.Songs == null || !album.Songs.Any())
                {
                    Write("    That's curious, there are no songs found for this album");
                }
                else
                {
                    foreach (var song in album.Songs.OrderBy(s => s.Position))
                    {
                        Write($"    {(song.Position.HasValue ? song.Position.ToString() : "-")}. {song.Title}");
                    }
                }
            }
        }

        private static int GetNumber(string text, int min, int max)
        {
            Write(text);
            var input = Console.ReadLine();
            var number = 0;
            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out number) || number < min || number > max)
            {
                Error($"Oops, you need to type a number between {min} and {max}! Please try again:");
                input = Console.ReadLine();
            }
            if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out number) || number < min || number > max)
            {
                Error("Nope, still not right. That's the end of your ride.");
                End();
            }
            return number;
        }

        private static string GetString(string text, List<string> acceptableAnswers = null)
        {
            Write(text);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Error("Oops, you need to type something! Please try again:");
                input = Console.ReadLine();
            }
            if (acceptableAnswers != null && acceptableAnswers.Any() && !acceptableAnswers.Contains(input))
            {
                Error($"The acceptable answers are: {string.Join(", ", acceptableAnswers)}. Please try again:");
                input = Console.ReadLine();
            }
            if (string.IsNullOrWhiteSpace(input) || (acceptableAnswers != null && acceptableAnswers.Any() && !acceptableAnswers.Contains(input)))
            {
                Error("Nope, still not right. That's the end of your ride.");
                End();
            }
            return input;
        }

        private static void Write(string text, bool extraLine = false)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
            if (extraLine)
            {
                Console.WriteLine();
            }
        }

        private static void Error(string text, bool extraLine = false)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = color;
            if (extraLine)
            {
                Console.WriteLine();
            }
        }

        private static void Success(string text, bool extraLine = false)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ForegroundColor = color;
            if (extraLine)
            {
                Console.WriteLine();
            }
        }

        private static void WriteLogo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   _                  _  ______      _             _                         ");
            Console.WriteLine("  | |                (_)/ _____)    | |           | |      _                 ");
            Console.WriteLine("  | |     _   _  ____ _| /      ____| | ____ _   _| | ____| |_  ___   ____   ");
            Console.WriteLine("  | |    | | | |/ ___) | |     / _  | |/ ___) | | | |/ _  |  _)/ _ \\ / ___)  ");
            Console.WriteLine("  | |____| |_| | |   | | \\____( ( | | ( (___| |_| | ( ( | | |_| |_| | |      ");
            Console.WriteLine("  |_______)__  |_|   |_|\\______)_||_|_|\\____)\\____|_|\\_||_|\\___)___/|_|      ");
            Console.WriteLine("         (____/                                                              ");
            Console.WriteLine("                                                                             ");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
