using System.Collections.Generic;
using Knapcode.RemindMeWhen.Core.Models;

namespace RemindMeWhen.Website.Models
{
    public class SearchResultsViewModel
    {
        public IEnumerable<MovieReleasedEvent> Events { get; set; }
    }
}