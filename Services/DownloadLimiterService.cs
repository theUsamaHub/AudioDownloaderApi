public class DownloadLimiterService
{
    // Only 3 concurrent jobs allowed
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(3, 3);

    public async Task<T> RunWithLimit<T>(Func<Task<T>> action)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}