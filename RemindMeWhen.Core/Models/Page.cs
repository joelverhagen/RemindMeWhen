using System.Collections.Generic;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public class Page<T>
    {
        public IEnumerable<T> Entries { get; set; }
        public PageOffset Offset { get; set; }
        public bool HasNextPage { get; set; }
    }
}