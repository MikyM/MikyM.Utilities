using System.Collections.Concurrent;

namespace MikyM.Common.Utilities;


/// <summary>
/// Concurrent task queue that utilizes <see cref="SemaphoreSlim"/> and <see cref="ConcurrentTaskQueue"/> with <see cref="TaskCompletionSource"/>.
/// </summary>
public class ConcurrentTaskQueue
{
    private readonly SemaphoreSlim _semaphore;
    private readonly ConcurrentQueue<TaskCompletionSource<bool>> _queue = new();

    public ConcurrentTaskQueue()
        => _semaphore = new SemaphoreSlim(1);

    /// <summary>
    /// Enqueues an action asynchronously.
    /// </summary>
    /// <typeparam name="T">Service</typeparam>
    /// <param name="task">Action to enqueue</param>
    /// <returns>Created task</returns>
    public async Task<T> EnqueueAsync<T>(Func<Task<T>> task)
    {
        var tcs = new TaskCompletionSource<bool>();
        _queue.Enqueue(tcs);
        await _semaphore.WaitAsync().ContinueWith(t =>
        {
            if (!_queue.TryDequeue(out var popped)) return;

            popped.SetResult(true);
        });
        try
        {
            return await task();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Enqueues an action asynchronously.
    /// </summary>
    /// <param name="task">Action to enqueue</param>
    /// <returns>Created task</returns>
    public async Task EnqueueAsync(Func<Task> task)
    {
        var tcs = new TaskCompletionSource<bool>();
        _queue.Enqueue(tcs);
        await _semaphore.WaitAsync().ContinueWith(t =>
        {
            if (_queue.TryDequeue(out var popped))
                popped.SetResult(true);
        });
        try
        {
            await task();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
