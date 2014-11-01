using System.Collections.Generic;
using System.Threading;

namespace Knapcode.RemindMeWhen.WebApi.Models
{
    public class PageViewModel<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Entries { get; set; }
        public PageLinks Links {get; set; }
    }
}