using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public class Page<T>
    {
        public IEnumerable<T> Entries { get; set; }
        public int PageLimit { get; set; }
        public int PageNumber { get; set; }
        public bool HasNextPage { get; set; }
    }
}
