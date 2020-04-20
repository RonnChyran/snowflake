﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snowflake.Extensibility.Queueing
{
    /// <summary>
    /// A queue for long-existing <see cref="IAsyncEnumerable{T}"/> that represent a collection of long running resumable jobs.
    /// </summary>
    public interface IAsyncJobQueue
    {
        /// <summary>
        /// Checks whether or not a job source with the given GUID exists.
        /// </summary>
        /// <param name="jobId">The GUID of the job to check</param>
        /// <returns>Whether or not the job source still exists, or has already been disposed.</returns>
        bool HasJob(Guid jobId);

        /// <summary>
        /// Retrieves the Job IDs for jobs active in the job queue, meaning
        /// that they have active enumerators being evaluated in the queue.
        /// 
        /// This does not include undisposed, but inactive jobs.
        /// </summary>
        /// <returns>The job IDs for jobs active in the job queue.</returns>
        IEnumerable<Guid> GetActiveJobs();

        /// <summary>
        /// Retrieves the Job IDs for all jobs in the queue, regardless of their state, including
        /// undisposed, but inactive jobs.
        /// </summary>
        /// <returns>Gets jobs currently stored in the job queue</returns>
        IEnumerable<Guid> GetQueuedJobs();

        /// <summary>
        /// Retrieves the job IDs for all jobs that are undisposed, but no longer have items 
        /// in the enumerator.
        /// </summary>
        /// <returns>he job IDs for jobs that are undisposed, but no longer have items left in their enumerator.</returns>
        IEnumerable<Guid> GetZombieJobs();


    }

    /// <summary>
    /// A queue for long-existing <see cref="IAsyncEnumerable{T}"/> that represent a collection of long running resumable jobs.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    public interface IAsyncJobQueue<T>
        : IAsyncJobQueue<IAsyncEnumerable<T>, T>, IAsyncJobQueue
    {
        /// <summary>
        /// Tries to remove the <see cref="IAsyncEnumerable{T}"/> as it was added to the job queue.
        /// This will only succeed if there are no active jobs for the enumerable in the queue.
        /// </summary>
        /// <param name="jobId">The jobId</param>
        /// <param name="asyncEnumerable">
        /// The async enumerable that is removed, or the empty enumerable if the result is false.
        /// This is unlike <see cref="IAsyncJobQueue{TAsyncEnumerable, T}.TryRemoveSource(Guid, out TAsyncEnumerable)"/>,
        /// which will return null instead of a enumerable.
        /// </param>
        /// <returns>If the enumerable was successfully removed.</returns>
        new bool TryRemoveSource(Guid jobId, out IAsyncEnumerable<T> asyncEnumerable);
    }

    /// <summary>
    /// A queue for long-existing <see cref="IAsyncEnumerable{T}"/> that represent a collection of long running resumable jobs.
    /// This variant allows for higher-kinded implementations of <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <typeparam name="TAsyncEnumerable">The type that implements <see cref="IAsyncEnumerable{T}"/></typeparam>

    public interface IAsyncJobQueue<TAsyncEnumerable, T> : IAsyncJobQueue
        where TAsyncEnumerable : class, IAsyncEnumerable<T>
    {
        /// <summary>
        /// Retrieves the current value in the enumerator and whether or not the enumerator is exhausted.
        /// Once the enumerator is exhausted, the value field will always be default(<typeparamref name="T"/>),
        /// and hasNext will be false.
        /// </summary>
        /// <param name="jobId">The job token that was returned by <see cref="QueueJob(TAsyncEnumerable, CancellationToken)"/></param>
        /// <returns>
        /// The next value, or null if the current value is null.
        /// </returns>
        T GetCurrent(Guid jobId);

        /// <summary>
        /// Retrieves the next value in the enumerator and whether or not the enumerator is exhausted.
        /// Once the enumerator is exhausted, the value field will always be default(<typeparamref name="T"/>),
        /// and hasNext will be false.
        /// </summary>
        /// <param name="jobId">The job token that was returned by <see cref="QueueJob(TAsyncEnumerable, CancellationToken)"/></param>
        /// <returns>
        /// The next value and whether or not to continue iterating.
        /// </returns>
        ValueTask<(T value, bool hasNext)> GetNext(Guid jobId);

        /// <summary>
        /// Gets the remaining values in the enumerator as an <see cref="IAsyncEnumerable{T}"/>
        /// that can be looped on.
        /// 
        /// Looping on this will exhaust the enumerator, and therefore using <see cref="GetNext(Guid)"/> after 
        /// enumerating the returned enumerable here will never return results.
        /// </summary>
        /// <param name="jobId">The job token that was returned by <see cref="QueueJob(TAsyncEnumerable, CancellationToken)"/></param>
        /// <returns>The remaining values in the enumerator as an <see cref="IAsyncEnumerable{T}"/></returns>
        IAsyncEnumerable<T> AsEnumerable(Guid jobId);

        /// <summary>
        /// Get the <see cref="IAsyncEnumerable{T}"/> as it was added to the job queue.
        /// </summary>
        /// <param name="jobId">The jobId</param>
        /// <returns>The <see cref="IAsyncEnumerable{T}"/> as it was added to the job queue.</returns>
        TAsyncEnumerable? GetSource(Guid jobId);

        /// <summary>
        /// Tries to remove the <see cref="IAsyncEnumerable{T}"/> as it was added to the job queue.
        /// This will only succeed if there are no active jobs for the enumerable in the queue,
        /// and it was not automatically disposed.
        /// 
        /// If not automatically disposed, this means that the enumerable
        /// must be exhausted before it can be removed.
        /// 
        /// </summary>
        /// <param name="jobId">The jobId</param>
        /// <param name="asyncEnumerable">The async enumerable that is removed, or null if the result is false.</param>
        /// <returns>If the enumerable was successfully removed.</returns>
        bool TryRemoveSource(Guid jobId, out TAsyncEnumerable? asyncEnumerable);

        /// <summary>
        /// Queues an <see cref="IAsyncEnumerable{T}"/> into the job queue.
        /// </summary>
        /// <param name="asyncEnumerable">The <see cref="IAsyncEnumerable{T}"/> to persist in memory.</param>
        /// <param name="token">A <see cref="CancellationToken"/> that will be passed to the enumerator used in <see cref="GetNext(Guid)"/>
        /// and <see cref="AsEnumerable(Guid)"/>.</param>
        /// <returns>A unique job token that can be used to modify the job at a later time.</returns>
        ValueTask<Guid> QueueJob(TAsyncEnumerable asyncEnumerable, CancellationToken token = default);

        /// <summary>
        /// Queues an <see cref="IAsyncEnumerable{T}"/> into the job queue with your own job token.
        /// If this job token already exists, this will replace the old job, which will be disposed.
        /// </summary>
        /// <param name="asyncEnumerable">The <see cref="IAsyncEnumerable{T}"/> to persist in memory.</param>
        /// <param name="guid">A token to refer to the job at a later time.</param>
        /// <param name="token">A <see cref="CancellationToken"/> that will be passed to the enumerator used in <see cref="GetNext(Guid)"/>
        /// and <see cref="AsEnumerable(Guid)"/>.</param>
        /// <returns>The <see cref="Guid"/> token you passed in <paramref name="guid"/>.</returns>
        ValueTask<Guid> QueueJob(TAsyncEnumerable asyncEnumerable, Guid guid, CancellationToken token = default);
    }
}
