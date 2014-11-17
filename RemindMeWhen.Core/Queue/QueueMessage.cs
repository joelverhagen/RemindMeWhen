using System;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class QueueMessage<T>
    {
        public string Id { get; set; }
        public string PopReceipt { get; set; }
        public DateTimeOffset? NextVisibleTime { get; set; }
        public T Content { get; set; }
    }
}