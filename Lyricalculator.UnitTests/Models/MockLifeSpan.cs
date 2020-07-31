using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;

namespace Lyricalculator.UnitTests.Models
{
    public class MockLifeSpan : ILifeSpan
    {
        public MockLifeSpan(DateTime begin, DateTime? end = null)
        {
            Begin = new PartialDate(begin.Year, begin.Month, begin.Day);
            if (end.HasValue)
            {
                End = new PartialDate(end.Value.Year, end.Value.Month, end.Value.Day);
            }
        }

        public PartialDate Begin { get; }

        public PartialDate End { get; }

        public bool Ended
        {
            get
            {
                return End != null && !End.IsEmpty;
            }
        }

        public IReadOnlyDictionary<string, object> UnhandledProperties => throw new NotImplementedException();
    }
}
