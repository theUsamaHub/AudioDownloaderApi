using System.Diagnostics;
using System.Text.Json;
namespace AudioDownloaderApi.Services
{
   public class ProcessService
    {
        private string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name.Length > 100 ? name.Substring(0, 100) : name;
        }

        public async Task<(string status, string title)> DownloadBestAudio(string url, string outputPath)
    {
        var durationProcess = new Process();
        durationProcess.StartInfo.FileName = "yt-dlp";
        durationProcess.StartInfo.Arguments = $"--no-playlist --dump-json \"{url}\"";
        durationProcess.StartInfo.RedirectStandardOutput = true;
        durationProcess.StartInfo.UseShellExecute = false;
        durationProcess.StartInfo.CreateNoWindow = true;

            durationProcess.StartInfo.RedirectStandardError = true;

            durationProcess.Start();
            string jsonOutput = await durationProcess.StandardOutput.ReadToEndAsync();
            string errorOutput = await durationProcess.StandardError.ReadToEndAsync();
            await durationProcess.WaitForExitAsync();

            if (string.IsNullOrWhiteSpace(jsonOutput))
            {
                // Log the errorOutput for debugging
                return ($"Failed to retrieve video info: {errorOutput}", null);
            }

            using var doc = JsonDocument.Parse(jsonOutput);

        int duration = doc.RootElement.GetProperty("duration").GetInt32();
        string title = doc.RootElement.GetProperty("title").GetString();

        if (duration > 900)
            return ("Video exceeds 15 minute limit.", null);

        var downloadProcess = new Process();
        downloadProcess.StartInfo.FileName = "yt-dlp";
        downloadProcess.StartInfo.Arguments =
            $"-f bestaudio --no-playlist -o \"{outputPath}\" \"{url}\"";
        downloadProcess.StartInfo.UseShellExecute = false;
        downloadProcess.StartInfo.CreateNoWindow = true;

        downloadProcess.Start();
        await downloadProcess.WaitForExitAsync();

        if (downloadProcess.ExitCode != 0)
            return ("Download failed.", null);

        return ("Download completed.", title);
    }
    public async Task<string> ConvertToMp3(string inputPath, string outputPath)
        {
            var process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = $"-i \"{inputPath}\" -vn -ab 192k -ar 44100 \"{outputPath}\"";
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            var exitTask = process.WaitForExitAsync();
            var completedTask = await Task.WhenAny(
                exitTask,
                Task.Delay(TimeSpan.FromMinutes(2))
            );

            if (completedTask != exitTask)
            {
                process.Kill();
                return "Conversion timed out.";
            }

            await exitTask;

            if (process.ExitCode != 0)
                return "Conversion failed.";

            return "Conversion completed.";
        }
        public async Task<(string status, string mp3Path, string downloadName)> DownloadAndConvertToMp3(string url)
        {
            string storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

            string internalName = Guid.NewGuid().ToString();

            string webmPath = Path.Combine(storagePath, internalName + ".webm");
            string mp3Path = Path.Combine(storagePath, internalName + ".mp3");

            var downloadResult = await DownloadBestAudio(url, webmPath);

            if (downloadResult.status != "Download completed.")
                return (downloadResult.status, null, null);
            long maxSizeBytes = 50 * 1024 * 1024; // 25MB

            if (File.Exists(webmPath))
            {
                var fileInfo = new FileInfo(webmPath);

                if (fileInfo.Length > maxSizeBytes)
                {
                    File.Delete(webmPath);
                    return ("File exceeds size limit (50MB).", null, null);
                }
            }
            var convertResult = await ConvertToMp3(webmPath, mp3Path);

            if (File.Exists(webmPath))
                File.Delete(webmPath);

            if (convertResult != "Conversion completed.")
                return (convertResult, null, null);

            string safeTitle = SanitizeFileName(downloadResult.title);

            return ("Success", mp3Path, safeTitle + ".mp3");
        }
    }

}
