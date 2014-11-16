using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface ISubscriptionStore
    {
        /// <summary>
        /// Get the subscription with the provided user ID and unique ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="uniqueId">The unique ID.</param>
        /// <returns>The subscription.</returns>
        Task<Subscription> GetSubscription(Guid userId, string uniqueId);

        /// <summary>
        /// Get all of the subscriptions belonging to the provided user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The subscriptions.</returns>
        Task<IEnumerable<Subscription>> GetUserSubscriptions(Guid userId);

        /// <summary>
        /// Get all of the subscriptions associated with the provided event ID.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <returns>The subscriptions.</returns>
        Task<IEnumerable<Subscription>> GetEventSubscriptions(EventId eventId);

        /// <summary>
        /// Save a new subscription for the provided event ID and user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="eventId">The event ID.</param>
        /// <returns>The new subscription.</returns>
        Task<Subscription> SaveSubscriptionAsync(Guid userId, EventId eventId);
    }
}