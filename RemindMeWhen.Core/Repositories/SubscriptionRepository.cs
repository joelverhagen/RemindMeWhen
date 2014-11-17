using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Queue;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IQueue<SaveSubscriptionMessage> _queue;
        private readonly ISubscriptionStore _subscriptionStore;

        public SubscriptionRepository(IQueue<SaveSubscriptionMessage> queue, ISubscriptionStore subscriptionStore)
        {
            _queue = queue;
            _subscriptionStore = subscriptionStore;
        }

        public async Task<Subscription> SaveSubscriptionAsync(Guid userId, EventId eventId)
        {
            // enqueue
            var message = new SaveSubscriptionMessage {UserId = userId, EventId = eventId};
            await _queue.AddMessageAsync(message, TimeSpan.Zero);

            // persist
            return await _subscriptionStore.SaveSubscriptionAsync(userId, eventId);
        }
    }
}
