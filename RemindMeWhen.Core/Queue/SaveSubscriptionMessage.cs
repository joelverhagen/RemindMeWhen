using System;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class SaveSubscriptionMessage
    {
        public EventId EventId { get; set; }
        public Guid UserId { get; set; }
    }
}