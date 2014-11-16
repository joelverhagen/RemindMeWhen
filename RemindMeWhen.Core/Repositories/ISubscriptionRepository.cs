using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public interface ISubscriptionRepository
    {
        /// <summary>
        /// Save a subscription for the provided user ID against the provided event ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="eventId">The event ID.</param>
        /// <returns>The resulting subscription.</returns>
        Task<Subscription> SaveSubscriptionAsync(Guid userId, EventId eventId);
    }
}