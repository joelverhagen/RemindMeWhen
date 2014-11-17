using System;
using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class SaveSubscriptionQueueJob : IQueueJob<SaveSubscriptionMessage>
    {
        public async Task ExecuteAsync(SaveSubscriptionMessage message)
        {
        }
    }
}
