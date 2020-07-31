using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;

namespace Lyricalculator.UnitTests.Models
{
    public class MockArtist : IArtist
    {
        public MockArtist(Guid id, string name, string country, MockLifeSpan lifeSpan)
        {
            Id = id;
            Name = name;
            Country = country;
            LifeSpan = lifeSpan;
        }

        public IArea Area => throw new NotImplementedException();

        public IArea BeginArea => throw new NotImplementedException();

        public string Country { get; }

        public IArea EndArea => throw new NotImplementedException();

        public string Gender => throw new NotImplementedException();

        public Guid? GenderId => throw new NotImplementedException();

        public IReadOnlyList<string> Ipis => throw new NotImplementedException();

        public IReadOnlyList<string> Isnis => throw new NotImplementedException();

        public ILifeSpan LifeSpan { get; }

        public IReadOnlyList<IRecording> Recordings => throw new NotImplementedException();

        public IReadOnlyList<IReleaseGroup> ReleaseGroups => throw new NotImplementedException();

        public IReadOnlyList<IRelease> Releases => throw new NotImplementedException();

        public string SortName => throw new NotImplementedException();

        public IReadOnlyList<IWork> Works => throw new NotImplementedException();

        public IReadOnlyList<IAlias> Aliases => throw new NotImplementedException();

        public string Annotation => throw new NotImplementedException();

        public string Disambiguation => throw new NotImplementedException();

        public string Name { get; }

        public IRating Rating => throw new NotImplementedException();

        public IRating UserRating => throw new NotImplementedException();

        public IReadOnlyList<IRelationship> Relationships => throw new NotImplementedException();

        public IReadOnlyList<IGenre> Genres => throw new NotImplementedException();

        public IReadOnlyList<ITag> Tags => throw new NotImplementedException();

        public IReadOnlyList<IGenre> UserGenres => throw new NotImplementedException();

        public IReadOnlyList<ITag> UserTags => throw new NotImplementedException();

        public string Type => throw new NotImplementedException();

        public Guid? TypeId => throw new NotImplementedException();

        public EntityType EntityType => throw new NotImplementedException();

        public Guid Id { get; }

        public IReadOnlyDictionary<string, object> UnhandledProperties => throw new NotImplementedException();
    }
}
