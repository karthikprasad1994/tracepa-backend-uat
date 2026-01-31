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
            IFormFile file, int docid)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Status = "Error", Message = "Please select a file to upload." });

            if (string.IsNullOrEmpty(folderPath))
                return BadRequest(new { Status = "Error", Message = "Folder path is required." });

            if (string.IsNullOrEmpty(userEmail))
                return BadRequest(new { Status = "Error", Message = "User email is required." });

            try
            {
                var result = await _driveService.UploadFileToFolderAsync(file, folderPath, userEmail, docid);

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
        public async Task<IActionResult> DeleteByDocId(
        int docId,
        [FromQuery] string userEmail)
        {
            if (docId <= 0)
            return StatusCode(500, new
            {
                message = "Invalid DocId",
            });

            if (string.IsNullOrWhiteSpace(userEmail))
            return StatusCode(500, new
            {
                message = "UserEmail is required",
            });

            try
            {
                await _driveService.DeleteFileByDocIdAsync(docId, userEmail);
                return Ok(new
                {
                    message = "File deleted successfully",
                    docId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        public class FolderStructureRequest
        {
            public string ParentFolderName { get; set; }
            public List<string> SubFolders { get; set; }
        }
        public class ExchangeCodeRequest
        {
            public string Code { get; set; }
            public string UserEmail { get; set; }
        }

        [HttpPost("exchange-code")]
        public async Task<IActionResult> ExchangeCode([FromBody] ExchangeCodeRequest request)
        {
            if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.UserEmail))
                return BadRequest("Code and UserEmail are required.");

            try
            {
                await _driveService.ExchangeCodeAsync(request.UserEmail, request.Code);
                return Ok(new { message = "Token saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("CheckDriveExist")]
        public async Task<IActionResult> CheckEmail([FromQuery] string gmail)
        {
            if (string.IsNullOrEmpty(gmail))
                return BadRequest(new { status = 0, message = "Gmail parameter is required" });

            int status = await _driveService.CheckEmailExistsAsync(gmail);

            return Ok(new { status });
        }


        // Add these methods to your existing GoogleDriveController

        [HttpGet("getfiles")]
        public async Task<IActionResult> GetFiles([FromQuery] string userEmail, [FromQuery] string folderPath = null)
        {
            try
            {
                var files = await _driveService.GetFilesAsync(userEmail, folderPath);
                return Ok(new
                {
                    Status = "Success",
                    Message = $"Retrieved {files.Count()} files successfully.",
                    Data = files
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("getfolders")]
        public async Task<IActionResult> GetFolders([FromQuery] string userEmail, [FromQuery] string parentFolderPath = null)
        {
            try
            {
                var folders = await _driveService.GetFoldersAsync(userEmail, parentFolderPath);
                return Ok(new
                {
                    Status = "Success",
                    Message = $"Retrieved {folders.Count()} folders successfully.",
                    Data = folders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        // Add these methods to your existing GoogleDriveController

        [HttpGet("getfilebyid")]
        public async Task<IActionResult> GetFileById([FromQuery] string userEmail, [FromQuery] int DocId)
        {
            if (DocId <= 0)
                return BadRequest(new { Status = "Error", Message = "Document Not Exist!" });

            try
            {
                var file = await _driveService.GetFileByIdAsync(DocId, userEmail);
                return Ok(new
                {
                    Status = "Success",
                    Message = "File retrieved successfully.",
                    Data = file
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("getfolderbyid")]
        public async Task<IActionResult> GetFolderById([FromQuery] string userEmail, [FromQuery] string folderId)
        {
            if (string.IsNullOrEmpty(folderId))
                return BadRequest(new { Status = "Error", Message = "Folder ID is required." });

            try
            {
                var folder = await _driveService.GetFolderByIdAsync(folderId, userEmail);
                return Ok(new
                {
                    Status = "Success",
                    Message = "Folder retrieved successfully.",
                    Data = folder
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("StorageIndication")]
        public async Task<IActionResult> GetStorageIndicator([FromQuery] string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest(new { Status = "Error", Message = "Missing userEmail parameter" });

            var result = await _driveService.GetStorageIndicatorAsync(userEmail);
            return Ok(result);
        }
    }
}
