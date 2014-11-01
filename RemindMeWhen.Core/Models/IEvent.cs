using System;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public interface IEvent
    {
        EventIdentity Identity { get; }
        DateTime? DateTime { get; }
    }
}