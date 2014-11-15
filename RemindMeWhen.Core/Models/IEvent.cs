using System;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public interface IEvent
    {
        EventId Id { get; }
        DateTime? DateTime { get; }
    }
}