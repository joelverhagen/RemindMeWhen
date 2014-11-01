using System.Collections.Generic;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models
{
    public class Actor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Characters { get; set; }
    }
}