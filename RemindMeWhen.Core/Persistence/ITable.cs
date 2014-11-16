using System.Collections.Generic;
using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface ITable<T>
    {
        /// <summary>
        /// Get the value with the provided partition and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key for the value.</param>
        /// <param name="rowKey">The row key for the value.</param>
        /// <returns>The value.</returns>
        Task<T> GetAsync(string partitionKey, string rowKey);

        /// <summary>
        /// Get a list of values with the provided partition key and a row key less than and equal to the row key
        /// upper bound and a greater than or equal to the row key lower bound.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKeyLowerBound">The lower bound on the row key (inclusive).</param>
        /// <param name="rowKeyUpperBound">The upper bound on the row key (inclusive).</param>
        /// <returns>The list of values.</returns>
        Task<IEnumerable<T>> ListAsync(string partitionKey, string rowKeyLowerBound, string rowKeyUpperBound);

        /// <summary>
        /// Set a value with the provided partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The task.</returns>
        Task SetAsync(string partitionKey, string rowKey, T value);
    }
}