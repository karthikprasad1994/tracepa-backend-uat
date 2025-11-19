using Microsoft.AspNetCore.Mvc;
using TracePA.Interfaces;

namespace TracePA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropboxController : ControllerBase
    {
        private readonly IDropboxService _dropboxService;

        public DropboxController(IDropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }

        [HttpGet("authorize")]
        public async Task<IActionResult> AuthorizeDropbox()
        {
            var url = await _dropboxService.GetAuthorizationUrlAsync();
            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var accessToken = await _dropboxService.ProcessCallbackAsync(code);
            return Ok(new { accessToken });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string accessToken, [FromQuery] string folder = "")
        {
            var fileId = await _dropboxService.UploadFileAsync(file, accessToken, folder);
            return Ok(new { fileId });
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListFiles([FromQuery] string accessToken, [FromQuery] string folder = "")
        {
            var files = await _dropboxService.ListFilesAsync(accessToken, folder);
            return Ok(files);
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string accessToken, [FromQuery] string filePath)
        {
            var stream = await _dropboxService.DownloadFileAsync(accessToken, filePath);
            return File(stream, "application/octet-stream", Path.GetFileName(filePath));
        }
    }
}
