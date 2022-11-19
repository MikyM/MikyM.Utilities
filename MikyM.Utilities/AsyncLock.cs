namespace MikyM.Utilities;

/// <summary>
/// Asynchronous lock.
/// </summary>
[PublicAPI]
public class AsyncLock
{
    private readonly Task<IDisposable> _releaserTask;
    private readonly SemaphoreSlim _semaphore = new (1, 1);
    private readonly IDisposable _releaser;

    /// <summary>
    /// 
    /// </summary>
    public AsyncLock()
    {
        _releaser = new Releaser(_semaphore);
        _releaserTask = Task.FromResult(_releaser);
    }

    /// <summary>
    /// Locks resource synchronously
    /// </summary>
    /// <returns>Releaser</returns>
    /// 
    public IDisposable Lock()
    {
        _semaphore.Wait();
        return _releaser;
    }

    /// <summary>
    /// Locks resource asynchronously
    /// </summary>
    /// <returns>Releaser task</returns>
    public Task<IDisposable> LockAsync()
    {
        var waitTask = _semaphore.WaitAsync();
        return waitTask.IsCompleted
            ? _releaserTask
            : waitTask.ContinueWith(
                (_, releaser) => (IDisposable)releaser!,
                _releaser,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default)!;
    }

    private class Releaser : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        public Releaser(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }
        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}
