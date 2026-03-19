using System.ComponentModel.DataAnnotations;

namespace AudioDownloaderApi.Models
{
    public class DownloadAudioRequest
    {
        [Required]
        [Url]
        public string Url { get; set; }

        // Optional: for validation before hitting yt-dlp
        [Range(1, 50, ErrorMessage = "Maximum file size allowed is 50MB.")]
        public int? MaxFileSizeMB { get; set; } = 50;

        [Range(1, 15, ErrorMessage = "Maximum video duration allowed is 15 minutes.")]
        public int? MaxDurationMinutes { get; set; } = 15;
    }
}