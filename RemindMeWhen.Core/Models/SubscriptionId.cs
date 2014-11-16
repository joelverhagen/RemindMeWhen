using System;

namespace Knapcode.RemindMeWhen.Core.Models
{
    public class SubscriptionId
    {
        public EventId EventId { get; set; }
        public Guid UserId { get; set; }
        public string UniqueId { get; set; }
    }
}