using MetaBrainz.MusicBrainz.Interfaces.Searches;

namespace Lyricalculator.UnitTests.Models
{
    public class MockSearchResult<T> : ISearchResult<T>
    {
        public MockSearchResult(T item, byte score = 1)
        {
            Item = item;
            Score = score;
        }

        public T Item { get; }

        public byte Score { get; }
    }
}
