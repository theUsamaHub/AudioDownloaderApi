using Microsoft.Extensions.Hosting;

namespace AudioDownloaderApi.Services
{
    public class FileCleanupService : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var storagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Storage"
                );

                if (Directory.Exists(storagePath))
                {
                    var files = Directory.GetFiles(storagePath);

                    foreach (var file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);
                            var lastWriteTime = fileInfo.LastWriteTime;

                            if (DateTime.Now - lastWriteTime > TimeSpan.FromMinutes(15)
                                && !IsFileLocked(file))
                            {
                                File.Delete(file);
                            }
                        }
                        catch
                        {
                            // ignore errors (locked, in-use, permission issues)
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private bool IsFileLocked(string path)
        {
            try
            {
                using var stream = File.Open(
                    path,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None
                );

                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}