namespace Knapcode.RemindMeWhen.Core.Models
{
    public class EventId
    {
        public EventType Type { get; set; }
        public ProviderId ProviderId { get; set; }
        public string ResourceId { get; set; }
    }
}
