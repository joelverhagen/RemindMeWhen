using System;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public abstract class MovieReleasedEvent : IEvent<MovieReleasedEventContent>
    {
        public EventId Id { get; set; }
        public DateTimeOffset? DateTimeOffset { get; set; }
        public MovieReleasedEventContent Content { get; set; }
    }
}