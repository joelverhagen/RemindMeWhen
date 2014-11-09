using System;
using Knapcode.RemindMeWhen.Core.Identities;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public abstract class MovieReleasedEvent : IEvent<MovieReleasedEventContent>
    {
        public EventIdentity Identity { get; set; }
        public DateTime? DateTime { get; set; }
        public MovieReleasedEventContent Content { get; set; }
    }
}