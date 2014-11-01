using System.Collections.Generic;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models
{
    public class Movie
    {
        public string Id { get; set; }
        public string MpaaRating { get; set; }
        public int? RunTime { get; set; }
        public string Synopsis { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public IEnumerable<Actor> AbridgedCast { get; set; }
        public ReleaseDates ReleaseDates { get; set; }
        public AlternateIds AlternateIds { get; set; }
        public MovieLinks Links { get; set; }
        public PosterLinks Posters { get; set; }
    }
}