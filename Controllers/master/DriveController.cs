using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TracePca.Interface.Master;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;
using TracePca.Service.Master;

namespace TracePca.Controllers.master
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleDriveController : ControllerBase
    {
        private readonly IGoogleDriveService _driveService;

        public GoogleDriveController(IGoogleDriveService driveService)
        {
            _driveService = driveService;
        }

        [HttpPost("create-folder-structure")]
        public async Task<IActionResult> CreateFolderStructure([FromQuery] string userEmail, [FromBody] FolderStructureRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ParentFolderName))
                return BadRequest("ParentFolderName is required.");

            var parentId = await _driveService.GetOrCreateFolderAsync(request.ParentFolderName, null, userEmail);
            var createdSubFolders = new List<object>();

            if (request.SubFolders?.Any() == true)
            {
                foreach (var sub in request.SubFolders)
                {
                    var folderId = await _driveService.GetOrCreateFolderAsync(sub, parentId, userEmail);
                    createdSubFolders.Add(new { name = sub, id = folderId });
                }
            }

            return Ok(new { success = true, parentFolder = new { name = request.ParentFolderName, id = parentId }, subFolders = createdSubFolders });
        }
        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile(
            [FromQuery] string userEmail,
            [FromQuery] string folderPath,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Status = "Error", Message = "Please select a file to upload." });

            if (string.IsNullOrEmpty(folderPath))
                return BadRequest(new { Status = "Error", Message = "Folder path is required." });

            if (string.IsNullOrEmpty(userEmail))
                return BadRequest(new { Status = "Error", Message = "User email is required." });

            try
            {
                var result = await _driveService.UploadFileToFolderAsync(file, folderPath, userEmail);

                // Try to extract Status property dynamically
                var statusProp = result?.GetType().GetProperty("Status");
                var statusValue = statusProp?.GetValue(result)?.ToString();

                if (statusValue == "Error")
                    return NotFound(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }


        [HttpGet("list")]
        public async Task<IActionResult> ListFiles([FromQuery] string userEmail)
        {
            var files = await _driveService.ListFilesAsync(userEmail);
            return Ok(files);
        }

        [HttpPost("rename")]
        public async Task<IActionResult> RenameFile([FromQuery] string userEmail, [FromQuery] string fileId, [FromQuery] string newName)
        {
            var file = await _driveService.RenameFileAsync(fileId, newName, userEmail);
            return Ok(file);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile([FromQuery] string userEmail, [FromQuery] string fileId)
        {
            await _driveService.DeleteFileAsync(fileId, userEmail);
            return Ok(new { success = true });
        }

        public class FolderStructureRequest
        {
            public string ParentFolderName { get; set; }
            public List<string> SubFolders { get; set; }
        }
    }
}
