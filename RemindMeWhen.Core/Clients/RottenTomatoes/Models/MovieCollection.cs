using System.Collections.Generic;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models
{
    public class MovieCollection
    {
        public int Total { get; set; }
        public string LinkTemplate { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
    }
}