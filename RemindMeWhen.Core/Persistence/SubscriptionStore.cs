using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Extensions;
using Knapcode.RemindMeWhen.Core.Hashing;
using Knapcode.RemindMeWhen.Core.Logging;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class SubscriptionStore : ISubscriptionStore
    {
        private readonly IEventSource _eventSource;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly ITable<Subscription> _table;

        public SubscriptionStore(IEventSource eventSource, IHashAlgorithm hashAlgorithm, ITable<Subscription> table)
        {
            _eventSource = eventSource;
            _hashAlgorithm = hashAlgorithm;
            _table = table;
        }

        public async Task<Subscription> GetSubscription(Guid userId, string uniqueId)
        {
            Subscription subscription = await _table.GetAsync(GetQueryByUserPartitionKey(userId), uniqueId);
            if (subscription == null)
            {
                _eventSource.OnMissingSubscriptionFromSubscriptionStore(userId, uniqueId);
                return null;
            }

            return subscription;
        }

        public async Task<IEnumerable<Subscription>> GetUserSubscriptions(Guid userId)
        {
            return await _table.ListAsync(GetQueryByUserPartitionKey(userId), null, null);
        }

        public async Task<IEnumerable<Subscription>> GetEventSubscriptions(EventId eventId)
        {
            return await _table.ListAsync(GetQueryByEventPartitionKey(eventId), null, null);
        }

        public async Task<Subscription> SaveSubscriptionAsync(Guid userId, EventId eventId)
        {
            // create the subscription
            DateTimeOffset created = DateTimeOffset.UtcNow;
            string uniqueId = string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}",
                created.GetDescendingOrderString(),
                Guid.NewGuid());

            var subscription = new Subscription
            {
                Id = new SubscriptionId
                {
                    EventId = eventId,
                    UserId = userId,
                    UniqueId = uniqueId,
                },
                Created = created
            };

            string rowKey = uniqueId;

            // save the subscription for querying by user
            string queryByUserPartitionKey = GetQueryByUserPartitionKey(userId);
            await _table.SetAsync(queryByUserPartitionKey, rowKey, subscription);

            // save the subscription for querying by event
            string queryByEventPartitionKey = GetQueryByEventPartitionKey(eventId);
            await _table.SetAsync(queryByEventPartitionKey, rowKey, subscription);

            return subscription;
        }

        private static string GetQueryByUserPartitionKey(Guid userId)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}",
                "user",
                userId);
        }

        private string GetQueryByEventPartitionKey(EventId eventId)
        {
            string raw = string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}-{2}",
                eventId.Type,
                eventId.ProviderId,
                eventId.ResourceId);
            string hash = _hashAlgorithm.GetHash(Encoding.UTF8.GetBytes(raw));

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}",
                "event",
                hash);
        }
    }
}