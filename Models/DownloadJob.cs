using System.Threading.Tasks;

namespace AudioDownloaderApi.Models
{
    

    public class DownloadJob
    {
        public string Url { get; set; }
        public int MaxFileSizeMB { get; set; }
        public int MaxDurationMinutes { get; set; }
        public TaskCompletionSource<(string status, string mp3Path, string downloadName)> Tcs { get; set; }
    }
}
