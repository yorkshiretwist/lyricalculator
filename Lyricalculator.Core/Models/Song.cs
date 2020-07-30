using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;

namespace Lyricalculator.Core.Models
{
    /// <summary>
    /// Represents a song
    /// </summary>
    public class Song
    {
        public string Title { get; set; }

        public Guid Id { get; set; }

        public TimeSpan? Length { get; set; }

        public int? Position { get; set; }

        public string Lyrics { get; set; }

        public LyricsStats LyricsStats { get; set; }

        public LyricsStatus LyricsStatus { get; set; }

        public override string ToString()
        {
            return Title;
        }

        /// <summary>
        /// For a "proper" system I'd use Automapper and abstract this class further from the MusicBrainz model
        /// </summary>
        /// <param name="release"></param>
        /// <returns></returns>
        public static Song FromMusicBrainzModel(ITrack track)
        {
            return new Song
            {
                Id = track.Id,
                Title = track.Title,
                Length = track.Length,
                Position = track.Position
            };
        }
    }
}