using System.Collections.Generic;

namespace Lyricalculator.Core.Models
{
    /// <summary>
    /// Represents a generic search response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchResponse<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
    }
}
