using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;

namespace Lyricalculator.Core.Models
{
    /// <summary>
    /// Represents an artist
    /// </summary>
    public class Artist
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public Lifespan Lifespan { get; set; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// For a "proper" system I'd use Automapper and abstract this class further from the MusicBrainz model
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        public static Artist FromMusicBrainzModel(IArtist artist)
        {
            return new Artist
            {
                Id = artist.Id,
                Name = artist.Name,
                Country = artist.Country,
                Lifespan = new Lifespan
                {
                    BeginYear = artist.LifeSpan?.Begin?.Year,
                    EndYear = artist.LifeSpan?.End?.Year
                }
            };
        }
    }
}