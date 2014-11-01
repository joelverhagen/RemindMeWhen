using System;
using System.Collections.Generic;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public class MovieReleaseEvent
    {
        public MovieReleaseEvent(MovieReleaseType type)
        {
            Type = type;
        }

        public string Title { get; set; }
        public int? Year { get; set; }
        public Uri ImdbUrl { get; set; }
        public Uri RottenTomatoesUrl { get; set; }
        public Uri ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public MovieReleaseType Type { get; private set; }
        public IEnumerable<string> Actors { get; set; }
    }
}
