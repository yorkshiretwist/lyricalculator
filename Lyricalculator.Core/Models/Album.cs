using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;

namespace Lyricalculator.Core.Models
{
    /// <summary>
    /// Represents an album
    /// </summary>
    public class Album
    {
        public string Title { get; set; }

        public Guid ReleaseGroupId { get; set; }

        public Guid ReleaseId { get; set; }

        public int? Year { get; set; }

        public IEnumerable<Song> Songs { get; set; }

        public override string ToString()
        {
            return Title;
        }

        /// <summary>
        /// For a "proper" system I'd use Automapper and abstract this class further from the MusicBrainz model
        /// </summary>
        /// <param name="releaseGroup"></param>
        /// <returns></returns>
        public static Album FromMusicBrainzModel(IReleaseGroup releaseGroup)
        {
            return new Album
            {
                ReleaseGroupId = releaseGroup.Id,
                Title = releaseGroup.Title,
                Year = releaseGroup.FirstReleaseDate?.Year
            };
        }
    }
}