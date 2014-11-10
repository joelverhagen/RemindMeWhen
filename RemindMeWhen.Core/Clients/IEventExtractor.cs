using System.Collections.Generic;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public interface IEventExtractor<in TSource, out TEvent> where TEvent : IEvent
    {
        IEnumerable<TEvent> Extract(TSource source);
    }
}