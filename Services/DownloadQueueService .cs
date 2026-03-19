using AudioDownloaderApi.Models;
using AudioDownloaderApi.Services;
using System.Threading.Channels;

public class DownloadQueueService : BackgroundService
{
    private readonly Channel<DownloadJob> _queue;
    private readonly ProcessService _process;
    private readonly int _maxConcurrency;

    public DownloadQueueService(ProcessService process, int maxConcurrency = 3)
    {
        _queue = Channel.CreateUnbounded<DownloadJob>();
        _process = process;
        _maxConcurrency = maxConcurrency;
    }

    public async Task<(string status, string mp3Path, string downloadName)> EnqueueJob(DownloadAudioRequest request)
    {
        var tcs = new TaskCompletionSource<(string, string, string)>();
        await _queue.Writer.WriteAsync(new DownloadJob
        {
            Url = request.Url,
            MaxFileSizeMB = request.MaxFileSizeMB ?? 50,
            MaxDurationMinutes = request.MaxDurationMinutes ?? 15,
            Tcs = tcs
        });

        return await tcs.Task;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var runningTasks = new List<Task>();

        await foreach (var job in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            // Wait if running tasks exceed max concurrency
            while (runningTasks.Count >= _maxConcurrency)
            {
                var finished = await Task.WhenAny(runningTasks);
                runningTasks.Remove(finished);
            }

            var task = Task.Run(async () =>
            {
                try
                {
                    var result = await _process.DownloadAndConvertToMp3(job.Url);
                    // Optional: enforce max size/duration here using your existing checks in ProcessService
                    job.Tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    job.Tcs.SetResult(("Failed: " + ex.Message, null, null));
                }
            });

            runningTasks.Add(task);
        }
    }
}