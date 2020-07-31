namespace Lyricalculator.Core.Models
{
    /// <summary>
    /// Represents a response when fetching lyrics
    /// </summary>
    public class LyricsResponse
    {
        public LyricsStatus Status { get; set; }

        public string Lyrics { get; set; }
    }
}
