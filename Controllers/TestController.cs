//using AudioDownloaderApi.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace AudioDownloaderApi.Controllers
//{
//    [ApiController]
//    [Route("api/test")]
//    public class TestController : ControllerBase
//    {
//        private readonly ProcessService _process;

//        public TestController(ProcessService process)
//        {
//            _process = process;
//        }

//        //[HttpGet]
//        //public async Task<IActionResult> Test()
//        //{
//        //    var result = await _process.RunCommand("yt-dlp", "--version");
//        //    return Ok(result);
//        //}
//        [HttpGet("download")]
//        public async Task<IActionResult> Download()
//        {
//            string url = "https://youtu.be/0aUJjBpLCX8";

//            string filePath = Path.Combine(
//                Directory.GetCurrentDirectory(),
//                "Storage",
//                "test.%(ext)s"
//            );

//            var result = await _process.DownloadBestAudio(url, filePath);

//            return Ok(result);
//        }
//        [HttpPost("download-audio")]
//        public async Task<IActionResult> DownloadAudio([FromBody] string url)
//        {
//            string fileName = Guid.NewGuid().ToString(); // unique file name
//            var result = await _process.DownloadAndConvertToMp3(url, fileName);

//            if (result != "Success")
//                return BadRequest(result);

//            string mp3Path = Path.Combine(Directory.GetCurrentDirectory(), "Storage", fileName + ".mp3");

//            return PhysicalFile(mp3Path, "audio/mpeg", fileName + ".mp3");
//        }
//    }
//}
