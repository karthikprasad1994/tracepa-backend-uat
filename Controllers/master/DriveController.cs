using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace TracePca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleDriveController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly string CredentialsPath = @"C:\Users\MMCS\Downloads\GoogleDrive1.json";
        private readonly string TokenFolder = "tokens";
        private readonly string[] Scopes = { DriveService.Scope.DriveFile, DriveService.Scope.Drive };
        public GoogleDriveController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // ----------------- Database Connection -----------------
        private string GetConnectionStringFromSession()
        {
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode missing in session.");
            return _configuration.GetConnectionString(dbName);
        }

        // ----------------- Logging -----------------
        private async Task<long> InsertMasterLogAsync(string userEmail, string actionType, string folderId, string folderName, string fileId, string fileName, string status, string message)
        {
            using var connection = new SqlConnection(GetConnectionStringFromSession());
            string sql = @"
                INSERT INTO DriveMasterLog 
                (UserEmail, ActionType, FolderId, FolderName, FileId, FileName, Status, Message, CreatedOn)
                VALUES (@UserEmail, @ActionType, @FolderId, @FolderName, @FileId, @FileName, @Status, @Message, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                UserEmail = userEmail,
                ActionType = actionType,
                FolderId = folderId ?? string.Empty,
                FolderName = folderName ?? string.Empty,
                FileId = fileId ?? string.Empty,
                FileName = fileName ?? string.Empty,
                Status = status,
                Message = message
            });
        }

        private async Task InsertDetailLogAsync(long masterLogId, string fileId, string fileName, long? fileSize, string status, string message)
        {
            using var connection = new SqlConnection(GetConnectionStringFromSession());
            string sql = @"
                INSERT INTO DriveDetailLog (MasterLogId, FileId, FileName, FileSize, Status, Message, CreatedOn)
                VALUES (@MasterLogId, @FileId, @FileName, @FileSize, @Status, @Message, GETDATE());";

            await connection.ExecuteAsync(sql, new
            {
                MasterLogId = masterLogId,
                FileId = fileId ?? string.Empty,
                FileName = fileName ?? string.Empty,
                FileSize = fileSize ?? 0,
                Status = status,
                Message = message
            });
        }

        // ----------------- Folder & File DB Save -----------------
        private async Task SaveFolderAsync(string folderId, string folderName, string parentFolderId, string userEmail, string mimeType)
        {
            using var connection = new SqlConnection(GetConnectionStringFromSession());
            string sql = @"
                IF NOT EXISTS (SELECT 1 FROM DriveFolders WHERE FolderId = @FolderId)
                BEGIN
                    INSERT INTO DriveFolders (FolderId, FolderName, ParentFolderId, UserEmail, MimeType, CreatedOn)
                    VALUES (@FolderId, @FolderName, @ParentFolderId, @UserEmail, @MimeType, GETDATE())
                END";
            await connection.ExecuteAsync(sql, new
            {
                FolderId = folderId,
                FolderName = folderName,
                ParentFolderId = parentFolderId,
                UserEmail = userEmail,
                MimeType = mimeType
            });
        }

        private async Task SaveFileAsync(string fileId, string fileName, string folderId, string userEmail, long size, string mimeType)
        {
            using var connection = new SqlConnection(GetConnectionStringFromSession());
            string sql = @"
                IF NOT EXISTS (SELECT 1 FROM DriveFiles WHERE FileId = @FileId)
                BEGIN
                    INSERT INTO DriveFiles (FileId, FileName, FolderId, UserEmail, Size, MimeType, CreatedOn)
                    VALUES (@FileId, @FileName, @FolderId, @UserEmail, @Size, @MimeType, GETDATE())
                END";
            await connection.ExecuteAsync(sql, new
            {
                FileId = fileId,
                FileName = fileName,
                FolderId = folderId,
                UserEmail = userEmail,
                Size = size,
                MimeType = mimeType
            });
        }

        // ----------------- Google Drive Service -----------------
        private async Task<DriveService> GetDriveServiceAsync(string userEmail)
        {
            using var stream = new FileStream(CredentialsPath, FileMode.Open, FileAccess.Read);
            string credPath = Path.Combine(TokenFolder, userEmail);

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                userEmail,
                CancellationToken.None,
                new Google.Apis.Util.Store.FileDataStore(credPath, true)
            );

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "TraceDriveIntegration"
            });
        }
        // ---------------- AES Keys ----------------
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("Your32CharLengthSecretKey1234567890"); // 32 chars = 256-bit
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("16CharInitVector"); // 16 chars = 128-bit

        // ---------------- AES Encryption ----------------
        private static void EncryptStream(Stream inputStream, Stream outputStream)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            inputStream.CopyTo(cryptoStream);
            cryptoStream.FlushFinalBlock();
        }

        // ---------------- AES Decryption ----------------
        private static void DecryptStream(Stream inputStream, Stream outputStream)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(outputStream);
            outputStream.Position = 0; // Reset position for reading
        }

        // ----------------- Folder Creation -----------------
        public class FolderStructureRequest
        {
            public string ParentFolderName { get; set; } = string.Empty;
            public List<string> SubFolderNames { get; set; } = new List<string>();
        }

        [HttpPost("create-parent-with-subfolders")]
        public async Task<IActionResult> CreateParentWithSubfolders([FromQuery] string userEmail, [FromBody] FolderStructureRequest request, [FromQuery] bool hidden = false)
        {
            if (string.IsNullOrEmpty(request.ParentFolderName))
                return BadRequest("ParentFolderName is required.");
            if (request.SubFolderNames == null || request.SubFolderNames.Count == 0)
                return BadRequest("SubFolderNames list cannot be empty.");

            var masterLogId = await InsertMasterLogAsync(userEmail, "CreateParentWithSubfolders", null, request.ParentFolderName, null, null, "Started", "Creating parent and subfolders");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);
                var createdItems = new List<object>();

                // --- STEP 1: Create Parent Folder under root ---
                string actualParentName = hidden ? $".{request.ParentFolderName}" : request.ParentFolderName;

                var parentListRequest = service.Files.List();
                parentListRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{actualParentName.Replace("'", "\\'")}' and 'root' in parents and trashed=false";
                parentListRequest.Fields = "files(id, name)";
                var parentListResponse = await parentListRequest.ExecuteAsync();
                var existingParent = parentListResponse.Files.FirstOrDefault();

                Google.Apis.Drive.v3.Data.File parentFolder;

                if (existingParent != null)
                {
                    parentFolder = existingParent;
                    createdItems.Add(new { parentFolder.Id, parentFolder.Name, Status = "Already Exists" });
                }
                else
                {
                    var parentMetadata = new Google.Apis.Drive.v3.Data.File
                    {
                        Name = actualParentName,
                        MimeType = "application/vnd.google-apps.folder",
                        Parents = new List<string> { "root" }
                    };

                    var parentCreateRequest = service.Files.Create(parentMetadata);
                    parentCreateRequest.Fields = "id, name";
                    parentFolder = await parentCreateRequest.ExecuteAsync();

                    await InsertMasterLogAsync(userEmail, "CreateFolder", parentFolder.Id, actualParentName, "root", null, "Success", "Parent folder created");
                }

                // Save parent folder in DB
                await SaveFolderAsync(parentFolder.Id, parentFolder.Name, null, userEmail, "application/vnd.google-apps.folder");

                // --- STEP 2: Create Subfolders inside Parent ---
                foreach (var subFolderName in request.SubFolderNames)
                {
                    string actualName = hidden ? $".{subFolderName}" : subFolderName;

                    var listRequest = service.Files.List();
                    listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{actualName.Replace("'", "\\'")}' and '{parentFolder.Id}' in parents and trashed=false";
                    listRequest.Fields = "files(id, name)";
                    var listResponse = await listRequest.ExecuteAsync();
                    var existingFolder = listResponse.Files.FirstOrDefault();

                    Google.Apis.Drive.v3.Data.File folder;
                    if (existingFolder != null)
                    {
                        folder = existingFolder;
                        createdItems.Add(new { folder.Id, folder.Name, Status = "Already Exists" });
                    }
                    else
                    {
                        var folderMetadata = new Google.Apis.Drive.v3.Data.File
                        {
                            Name = actualName,
                            MimeType = "application/vnd.google-apps.folder",
                            Parents = new List<string> { parentFolder.Id }
                        };
                        var createRequest = service.Files.Create(folderMetadata);
                        createRequest.Fields = "id, name";
                        folder = await createRequest.ExecuteAsync();
                        createdItems.Add(new { folder.Id, folder.Name, Status = "Created" });

                        await InsertMasterLogAsync(userEmail, "CreateFolder", folder.Id, actualName, parentFolder.Id, null, "Success", "Subfolder created");
                    }

                    // Save subfolder in DB
                    await SaveFolderAsync(folder.Id, folder.Name, parentFolder.Id, userEmail, "application/vnd.google-apps.folder");
                }

                return Ok(new
                {
                    ParentFolderId = parentFolder.Id,
                    ParentFolderName = parentFolder.Name,
                    Items = createdItems
                });
            }
            catch (Exception ex)
            {
                await InsertMasterLogAsync(userEmail, "CreateParentWithSubfolders", null, request.ParentFolderName, null, null, "Failed", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // ----------------- Delete Folder -----------------
        [HttpDelete("delete-folder")]
        public async Task<IActionResult> DeleteFolder([FromQuery] string userEmail, [FromQuery] string folderId)
        {
            var masterLogId = await InsertMasterLogAsync(userEmail, "DeleteFolder", folderId, null, null, null, "Started", "Deleting folder");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);
                await service.Files.Delete(folderId).ExecuteAsync();
                await InsertDetailLogAsync(masterLogId, null, null, null, "Success", "Folder deleted successfully");
                return Ok("Folder deleted successfully");
            }
            catch (Exception ex)
            {
                await InsertDetailLogAsync(masterLogId, null, null, null, "Failed", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromQuery] string userEmail, [FromQuery] string folderId, IFormFile file)
        {
            if (file == null) return BadRequest("File is required");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);

                // Read file into memory
                using var originalStream = new MemoryStream();
                await file.CopyToAsync(originalStream);
                originalStream.Position = 0;

                // Encrypt file
                var encryptedStream = new MemoryStream();
                EncryptStream(originalStream, encryptedStream);
                encryptedStream.Position = 0;

                // Upload to Google Drive
                var metadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = file.FileName,
                    Parents = new[] { folderId }
                };

                var request = service.Files.Create(metadata, encryptedStream, file.ContentType ?? "application/octet-stream");
                request.Fields = "id, name, size, mimeType";
                await request.UploadAsync();

                var uploaded = request.ResponseBody;

                return Ok(new { FileId = uploaded.Id, FileName = uploaded.Name, Size = uploaded.Size });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // ----------------- Archive Folder Recursively -----------------
        [HttpPost("archive-folder")]
        public async Task<IActionResult> ArchiveFolder([FromQuery] string userEmail, [FromQuery] string folderId, [FromQuery] string path)
        {
            var masterLogId = await InsertMasterLogAsync(userEmail, "ArchiveFolder", folderId, null, null, path, "Started", "Archiving folder");

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return BadRequest("Valid local folder path is required.");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);
                string rootFolderName = new DirectoryInfo(path).Name;

                var rootFolder = new Google.Apis.Drive.v3.Data.File
                {
                    Name = rootFolderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new[] { folderId }
                };
                var createRootRequest = service.Files.Create(rootFolder);
                createRootRequest.Fields = "id";
                var rootResponse = await createRootRequest.ExecuteAsync();

                // Save root folder in DB
                await SaveFolderAsync(rootResponse.Id, rootFolderName, folderId, userEmail, "application/vnd.google-apps.folder");

                // Recursive upload
                await UploadDirectoryRecursive(service, path, rootResponse.Id, userEmail, masterLogId);

                await InsertDetailLogAsync(masterLogId, rootResponse.Id, rootFolderName, 0, "Success", "Folder archived successfully");

                return Ok(new { rootResponse.Id, rootFolderName, Message = $"Folder '{rootFolderName}' uploaded successfully to Google Drive" });
            }
            catch (Exception ex)
            {
                await InsertDetailLogAsync(masterLogId, null, path, 0, "Failed", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        private async Task UploadDirectoryRecursive(DriveService service, string localPath, string parentFolderId, string userEmail, long masterLogId)
        {
            // Upload files
            foreach (var filePath in Directory.GetFiles(localPath))
            {
                var fileName = Path.GetFileName(filePath);

                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var metadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileName,
                    Parents = new[] { parentFolderId }
                };

                var request = service.Files.Create(metadata, stream, "application/octet-stream");
                request.Fields = "id, name, size, mimeType";
                await request.UploadAsync();

                var uploaded = request.ResponseBody;

                await SaveFileAsync(uploaded.Id, uploaded.Name, parentFolderId, userEmail, uploaded.Size ?? 0, uploaded.MimeType);
                await InsertDetailLogAsync(masterLogId, uploaded.Id, uploaded.Name, uploaded.Size ?? 0, "Success", "File uploaded");
            }

            // Upload subfolders recursively
            foreach (var subDir in Directory.GetDirectories(localPath))
            {
                var folderName = Path.GetFileName(subDir);

                var newFolder = new Google.Apis.Drive.v3.Data.File
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new[] { parentFolderId }
                };
                var createRequest = service.Files.Create(newFolder);
                createRequest.Fields = "id";
                var folderResponse = await createRequest.ExecuteAsync();

                await SaveFolderAsync(folderResponse.Id, folderName, parentFolderId, userEmail, "application/vnd.google-apps.folder");

                await UploadDirectoryRecursive(service, subDir, folderResponse.Id, userEmail, masterLogId);
            }
        }

        // ----------------- Other Methods -----------------
        [HttpGet("get-folder-contents")]
        public async Task<IActionResult> GetFolderContents([FromQuery] string userEmail, [FromQuery] string folderId)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(folderId))
                return BadRequest("userEmail and folderId are required.");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);
                var listRequest = service.Files.List();
                listRequest.Q = $"'{folderId}' in parents and trashed = false";
                listRequest.Fields = "files(id, name, mimeType, size, modifiedTime)";
                var response = await listRequest.ExecuteAsync();

                var result = response.Files.Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.MimeType,
                    f.Size,
                    f.ModifiedTime,
                    Type = f.MimeType == "application/vnd.google-apps.folder" ? "Folder" : "File"
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("get-file")]
        public async Task<IActionResult> GetFile([FromQuery] string userEmail, [FromQuery] string fileId, [FromQuery] bool download = false)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(fileId))
                return BadRequest("userEmail and fileId are required.");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);

                // Get file metadata
                var getRequest = service.Files.Get(fileId);
                getRequest.Fields = "id, name, mimeType, size, modifiedTime";
                var file = await getRequest.ExecuteAsync();

                if (download)
                {
                    // Download encrypted file
                    var encryptedStream = new MemoryStream();
                    await service.Files.Get(fileId).DownloadAsync(encryptedStream);
                    encryptedStream.Position = 0;

                    // Decrypt
                    var decryptedStream = new MemoryStream();
                    DecryptStream(encryptedStream, decryptedStream);
                    decryptedStream.Position = 0;

                    return File(decryptedStream, "application/octet-stream", file.Name);
                }

                return Ok(new { file.Id, file.Name, file.MimeType, file.Size, file.ModifiedTime });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("get-root-contents")]
        public async Task<IActionResult> GetRootContents([FromQuery] string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("userEmail is required.");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);
                var listRequest = service.Files.List();
                listRequest.Q = "'root' in parents and trashed = false";
                listRequest.Fields = "files(id, name, mimeType, size, modifiedTime)";
                var response = await listRequest.ExecuteAsync();

                return Ok(response.Files.Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.MimeType,
                    f.Size,
                    f.ModifiedTime,
                    Type = f.MimeType == "application/vnd.google-apps.folder" ? "Folder" : "File"
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("check-storage")]
        public async Task<IActionResult> CheckStorage([FromQuery] string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("userEmail is required.");

            try
            {
                var service = await GetDriveServiceAsync(userEmail);
                var aboutRequest = service.About.Get();
                aboutRequest.Fields = "storageQuota";
                var about = await aboutRequest.ExecuteAsync();

                double bytesToGb = 1024 * 1024 * 1024;
                return Ok(new
                {
                    UserEmail = userEmail,
                    StorageLimitGB = about.StorageQuota.Limit.HasValue ? Math.Round(about.StorageQuota.Limit.Value / bytesToGb, 2) : 0,
                    StorageUsageGB = about.StorageQuota.Usage.HasValue ? Math.Round(about.StorageQuota.Usage.Value / bytesToGb, 2) : 0,
                    StorageUsageInDriveGB = about.StorageQuota.UsageInDrive.HasValue ? Math.Round(about.StorageQuota.UsageInDrive.Value / bytesToGb, 2) : 0,
                    StorageUsageInDriveTrashGB = about.StorageQuota.UsageInDriveTrash.HasValue ? Math.Round(about.StorageQuota.UsageInDriveTrash.Value / bytesToGb, 2) : 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
