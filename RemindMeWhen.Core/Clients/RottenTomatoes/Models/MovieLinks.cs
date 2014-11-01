using System;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models
{
    public class MovieLinks : LinkCollection
    {
        public Uri Alternate { get; set; }
        public Uri Cast { get; set; }
        public Uri Reviews { get; set; }
        public Uri Similar { get; set; }
    }
}