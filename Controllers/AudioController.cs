//using AudioDownloaderApi.Models;
//using AudioDownloaderApi.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AudioDownloaderApi.Controllers
//{
//    [ApiController]
//    [Route("api/audio")]
//    public class AudioController : ControllerBase
//    {
//        private readonly ProcessService _process;
//        private readonly DownloadLimiterService _limiter;

//        public AudioController(ProcessService process, DownloadLimiterService limiter)
//        {
//            _process = process;
//            _limiter = limiter;
//        }

//        [HttpPost("download")]
//        public async Task<IActionResult> DownloadAudio([FromBody] DownloadAudioRequest request)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            // Use limiter and correct URL
//            var result = await _limiter.RunWithLimit(() => _process.DownloadAndConvertToMp3(request.Url));

//            if (result.status != "Success")
//                return BadRequest(result.status);

//            return PhysicalFile(
//                result.mp3Path,
//                "audio/mpeg",
//                result.downloadName
//            );
//        }a
//    }
//}

using AudioDownloaderApi.Models;
using AudioDownloaderApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AudioDownloaderApi.Controllers
{
    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        private readonly DownloadQueueService _queue;

        public AudioController(DownloadQueueService queue)
        {
            _queue = queue;
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadAudio([FromBody] DownloadAudioRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Enqueue request; will wait asynchronously until processed
            var result = await _queue.EnqueueJob(request);

            if (result.status != "Success")
                return BadRequest(result.status);

            return PhysicalFile(
                result.mp3Path,
                "audio/mpeg",
                result.downloadName
            );
        }
    }
}