using System.Collections.Generic;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Website.Models
{
    public class SearchParametersViewModel
    {
        public string EventType { get; set; }
        public string Query { get; set; }
        public IEnumerable<EventType> EventTypes { get; set; }
    }
}