using System;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public interface IEvent
    {
        EventId Id { get; }
        DateTimeOffset? DateTimeOffset { get; }
    }
}