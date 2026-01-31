using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Dapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TracePca.Interface.Master;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Util;

namespace TracePca.Service.Master
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private static readonly string[] Scopes = { DriveService.Scope.DriveFile, DriveService.Scope.Drive };

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;
        private readonly string _credentialsPath;
        private readonly string _logFilePath;
        private readonly bool _isDevelopment;

        public GoogleDriveService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _connectionString = GetConnectionStringFromSession();

            _isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            // localPath = @"D:\Steffi\Backend Project\tracepa-corebackend\client_secret_desktop.json";
            string localPath = @"\\MMCS-SERVER19\EMP_Backup\Googledrivetoken\client_secret_desktop.json";


            string cloudPath = @"C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\GoogleDrive\client_secret.json";
            _credentialsPath = _isDevelopment ? localPath : cloudPath;

            _logFilePath = _isDevelopment
                ? @"D:\Projects\Gitlab\TraceAPI - Backend Code\tracepa-dotnet-core\Logs\GoogleDriveLog.txt"
                : @"C:\inetpub\vhosts\multimedia.interactivedns.com\tracepacore.multimedia.interactivedns.com\Logs\GoogleDriveLog.txt";

            if (!File.Exists(_credentialsPath))
                throw new FileNotFoundException($"Google API credentials file not found at {_credentialsPath}");
        }

        private string GetConnectionStringFromSession()
        {
            var dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");
            if (string.IsNullOrWhiteSpace(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            var connStr = _configuration.GetConnectionString(dbName);
            if (string.IsNullOrWhiteSpace(connStr))
                throw new Exception($"Connection string for '{dbName}' not found in configuration.");

            return connStr;
        }

        #region Logging
        private void LogInfo(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
                File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [INFO] {message}{Environment.NewLine}");
            }
            catch { }
        }

        private void LogError(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
                File.AppendAllText(_logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] {message}{Environment.NewLine}");
            }
            catch { }
        }
        #endregion

        #region Drive Initialization
        public async Task<DriveService> GetDriveServiceAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                throw new ArgumentException("User email must be provided.", nameof(userEmail));

            UserCredential credential;

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = "SELECT TokenJson FROM UserDriveTokens WHERE UserEmail = @UserEmail";
                var tokenJson = await connection.QueryFirstOrDefaultAsync<string>(query, new { UserEmail = userEmail });

                using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);

                var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets,
                    Scopes = Scopes,
                    DataStore = new NullDataStore()
                });

                if (!string.IsNullOrEmpty(tokenJson))
                {
                    var token = JsonConvert.DeserializeObject<Google.Apis.Auth.OAuth2.Responses.TokenResponse>(tokenJson);
                    credential = new UserCredential(flow, userEmail, token);

                    // ✅ Check and refresh token if expired
                    if (credential.Token.IsExpired(SystemClock.Default))
                    {
                        LogInfo($"Token expired for {userEmail}, refreshing...");

                        bool success = await credential.RefreshTokenAsync(CancellationToken.None);
                        if (success)
                        {
                            string updateQuery = "UPDATE UserDriveTokens SET TokenJson = @TokenJson, UpdatedOn = @UpdatedOn WHERE UserEmail = @UserEmail";
                            await connection.ExecuteAsync(updateQuery, new
                            {
                                UserEmail = userEmail,
                                TokenJson = JsonConvert.SerializeObject(credential.Token),
                                UpdatedOn = DateTime.UtcNow
                            });

                            LogInfo($"Token refreshed and updated in DB for {userEmail}");
                        }
                        else
                        {
                            LogError($"Failed to refresh token for {userEmail}. Reauthorization required.");
                            throw new UnauthorizedAccessException("Google Drive token refresh failed. User needs to reauthorize.");
                        }
                    }
                    else
                    {
                        LogInfo($"Loaded valid Google Drive token from DB for {userEmail}");
                    }
                }
                else
                {
                    // No token found → trigger Google OAuth login
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        clientSecrets,
                        Scopes,
                        userEmail,
                        CancellationToken.None,
                        new FileDataStore("Tokens", true)
                    );

                    string insertQuery = @"
                INSERT INTO UserDriveTokens (UserEmail, TokenJson, CreatedOn)
                VALUES (@UserEmail, @TokenJson, @CreatedOn)";
                    await connection.ExecuteAsync(insertQuery, new
                    {
                        UserEmail = userEmail,
                        TokenJson = JsonConvert.SerializeObject(credential.Token),
                        CreatedOn = DateTime.UtcNow
                    });

                    LogInfo($"Saved new Google Drive token to DB for {userEmail}");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error creating DriveService for {userEmail}: {ex.Message}");
                throw;
            }

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "TracePA Drive Integration"
            });

            LogInfo($"DriveService successfully created for {userEmail}");
            return driveService;
        }

        #endregion

        #region Folder Operations
        public async Task<string> GetOrCreateFolderAsync(string folderName, string parentId, string userEmail)
        {
            var service = await GetDriveServiceAsync(userEmail);

            var listRequest = service.Files.List();
            listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and trashed=false";
            if (!string.IsNullOrEmpty(parentId))
                listRequest.Q += $" and '{parentId}' in parents";
            listRequest.Fields = "files(id, name)";
            var result = await listRequest.ExecuteAsync();

            if (result.Files.Any())
                return result.Files.First().Id;

            var folderMetadata = new GoogleDriveFile
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = string.IsNullOrEmpty(parentId) ? null : new[] { parentId }
            };

            var createRequest = service.Files.Create(folderMetadata);
            createRequest.Fields = "id";
            var folder = await createRequest.ExecuteAsync();

            // 🔹 Insert Folder record into UserDriveItemsNumeric table
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                string insertQuery = @"
                    INSERT INTO UserDriveItemsNumeric (UserEmail, FolderPath, FolderId, CreatedOn)
                    VALUES (@UserEmail, @FolderPath, @FolderId, GETUTCDATE());";

                await connection.ExecuteAsync(insertQuery, new
                {
                    UserEmail = userEmail,
                    FolderPath = folderName,
                    FolderId = folder.Id
                });
                LogInfo($"Folder '{folderName}' saved to UserDriveItemsNumeric table for {userEmail}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to insert folder info: {ex.Message}");
            }

            return folder.Id;
        }

        public async Task<string> GetFolderIdByPathAsync(string folderPath, string userEmail)
        {
            var service = await GetDriveServiceAsync(userEmail);
            string parentId = null;
            var folders = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            foreach (var folder in folders)
            {
                var listRequest = service.Files.List();
                listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folder}' and trashed=false";
                if (!string.IsNullOrEmpty(parentId))
                    listRequest.Q += $" and '{parentId}' in parents";
                listRequest.Fields = "files(id, name)";
                var result = await listRequest.ExecuteAsync();

                if (!result.Files.Any()) return null;
                parentId = result.Files.First().Id;
            }

            return parentId;
        }
        #endregion

        #region File Operations
        public async Task<object> UploadFileToFolderAsync(IFormFile file, string folderPath, string userEmail, int docid)
        {
            if (file == null || file.Length == 0)
                return new { Status = "Error", Message = "No file uploaded." };

            var service = await GetDriveServiceAsync(userEmail);
            var folderId = await GetFolderIdByPathAsync(folderPath, userEmail);

            if (folderId == null)
                return new { Status = "Error", Message = $"Folder path '{folderPath}' does not exist." };

            var fileMetadata = new GoogleDriveFile
            {
                Name = file.FileName,
                Parents = new[] { folderId }
            };

            using var stream = file.OpenReadStream();
            var request = service.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "id, name, parents, mimeType";
            await request.UploadAsync();

            // 🔹 Insert File record into UserDriveItemsNumeric table
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                string insertQuery = @"
                    INSERT INTO UserDriveItemsNumeric (UserEmail, FolderPath, FolderId, FileName, FileId, MimeType, CreatedOn,DocId)
                    VALUES (@UserEmail, @FolderPath, @FolderId, @FileName, @FileId, @MimeType, GETUTCDATE(), @DocId);";

                await connection.ExecuteAsync(insertQuery, new
                {
                    UserEmail = userEmail,
                    FolderPath = folderPath,
                    FolderId = folderId,
                    FileName = file.FileName,
                    FileId = request.ResponseBody.Id,
                    MimeType = request.ResponseBody.MimeType,
                    DocId = docid
                });
                LogInfo($"File '{file.FileName}' saved to UserDriveItemsNumeric table for {userEmail}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to insert file info: {ex.Message}");
            }

            return new
            {
                Status = "Success",
                Message = "File uploaded successfully.",
                File = new
                {
                    request.ResponseBody.Id,
                    request.ResponseBody.Name,
                    request.ResponseBody.Parents
                }
            };
        }

        public async Task<IEnumerable<GoogleDriveFile>> ListFilesAsync(string userEmail)
        {
            var service = await GetDriveServiceAsync(userEmail);
            var request = service.Files.List();
            request.Fields = "files(id, name, mimeType)";
            var result = await request.ExecuteAsync();
            return result.Files;
        }

        public async Task<GoogleDriveFile> RenameFileAsync(string fileId, string newName, string userEmail)
        {
            var service = await GetDriveServiceAsync(userEmail);
            var fileMetadata = new GoogleDriveFile { Name = newName };
            var request = service.Files.Update(fileMetadata, fileId);
            request.Fields = "id, name";
            var updatedFile = await request.ExecuteAsync();

            // 🔹 Update DB record in UserDriveItemsNumeric table
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                string updateQuery = "UPDATE UserDriveItemsNumeric SET FileName = @NewName, UpdatedOn = GETUTCDATE() WHERE FileId = @FileId";
                await connection.ExecuteAsync(updateQuery, new { NewName = newName, FileId = fileId });
                LogInfo($"File name updated in UserDriveItemsNumeric table for {fileId}");
            }
            catch (Exception ex)
            {
                LogError($"Failed to update file name in UserDriveItemsNumeric table: {ex.Message}");
            }

            return updatedFile;
        }

        public async Task DeleteFileByDocIdAsync(int docId, string userEmail)
        {
            string fileId;

            // 1️⃣ Get FileId from DB
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                fileId = await connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT FileId FROM UserDriveItemsNumeric WHERE DocId = @DocId",
                    new { DocId = docId });

                if (string.IsNullOrEmpty(fileId))
                    throw new Exception($"No FileId found for DocId {docId}");
            }

            // 2️⃣ Delete from Google Drive
            var service = await GetDriveServiceAsync(userEmail);
            await service.Files.Delete(fileId).ExecuteAsync();
            LogInfo($"Deleted Google Drive file: {fileId}");

            // 3️⃣ Delete DB records in transaction
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Child table first
                    await connection.ExecuteAsync(
                        "DELETE FROM EDT_attachments WHERE ATCH_DOCID = @DocId",
                        new { DocId = docId },
                        transaction);

                    // Parent table
                    await connection.ExecuteAsync(
                        "DELETE FROM UserDriveItemsNumeric WHERE DocId = @DocId",
                        new { DocId = docId },
                        transaction);

                    transaction.Commit();
                    LogInfo($"Deleted DB records for DocId {docId}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogError($"DB delete failed for DocId {docId}: {ex.Message}");
                    throw;
                }
            }
        }

        #endregion

        public async Task ExchangeCodeAsync(string userEmail, string code)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(code))
                throw new ArgumentException("UserEmail and Code are required.");

            var clientId = _configuration["Google:ClientId"];
            var clientSecret = _configuration["Google:ClientSecret"];
            var redirectUri = _configuration["Google:RedirectUri"];
            var scopes = new[] { Google.Apis.Drive.v3.DriveService.Scope.DriveFile };

            try
            {
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    Scopes = scopes
                });

                var tokenResponse = await flow.ExchangeCodeForTokenAsync(userEmail, code, redirectUri, CancellationToken.None);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string insertQuery = @"
                MERGE INTO UserDriveTokens AS target
                USING (SELECT @UserEmail AS UserEmail) AS source
                ON target.UserEmail = source.UserEmail
                WHEN MATCHED THEN 
                    UPDATE SET TokenJson = @TokenJson, UpdatedOn = GETUTCDATE()
                WHEN NOT MATCHED THEN
                    INSERT (UserEmail, TokenJson, CreatedOn) VALUES (@UserEmail, @TokenJson, GETUTCDATE());";

                await connection.ExecuteAsync(insertQuery, new
                {
                    UserEmail = userEmail,
                    TokenJson = JsonConvert.SerializeObject(tokenResponse)
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exchanging code for {userEmail}: {ex.Message}", ex);
            }
        }

        public async Task<int> CheckEmailExistsAsync(string gmail)
        {
            if (string.IsNullOrEmpty(gmail))
                return 0;
            gmail = GetUserEmail();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var exists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM UserDriveTokens WHERE UserEmail = @Gmail",
                new { Gmail = gmail }
            );

            return exists > 0 ? 1 : 0;
        }


        // Add these methods to your existing GoogleDriveService class

        public async Task<IEnumerable<GoogleDriveFile>> GetFilesAsync(string userEmail, string folderPath = null)
        {
            userEmail = GetUserEmail();
            var service = await GetDriveServiceAsync(userEmail);

            try
            {
                string folderId = null;

                if (!string.IsNullOrEmpty(folderPath))
                {
                    folderId = await GetFolderIdByPathAsync(folderPath, userEmail);
                    if (folderId == null)
                    {
                        LogError($"Folder path '{folderPath}' not found for user {userEmail}");
                        return new List<GoogleDriveFile>();
                    }
                }

                var request = service.Files.List();
                var queryParts = new List<string>
        {
            "mimeType != 'application/vnd.google-apps.folder'",
            "trashed = false"
        };

                if (!string.IsNullOrEmpty(folderId))
                {
                    queryParts.Add($"'{folderId}' in parents");
                }

                request.Q = string.Join(" and ", queryParts);
                request.Fields = "files(id, name, mimeType, size, createdTime, modifiedTime, webViewLink, webContentLink, parents)";
                request.PageSize = 1000;

                var result = await request.ExecuteAsync();
                LogInfo($"Retrieved {result.Files.Count} files for user {userEmail} from folder: {folderPath ?? "root"}");

                return result.Files;
            }
            catch (Exception ex)
            {
                LogError($"Error getting files for user {userEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<GoogleDriveFile>> GetFoldersAsync(string userEmail, string parentFolderPath = null)
        {
            userEmail = GetUserEmail();
            var service = await GetDriveServiceAsync(userEmail);

            try
            {
                string parentFolderId = null;

                if (!string.IsNullOrEmpty(parentFolderPath))
                {
                    parentFolderId = await GetFolderIdByPathAsync(parentFolderPath, userEmail);
                    if (parentFolderId == null)
                    {
                        LogError($"Parent folder path '{parentFolderPath}' not found for user {userEmail}");
                        return new List<GoogleDriveFile>();
                    }
                }

                var request = service.Files.List();
                var queryParts = new List<string>
        {
            "mimeType = 'application/vnd.google-apps.folder'",
            "trashed = false"
        };

                if (!string.IsNullOrEmpty(parentFolderId))
                {
                    queryParts.Add($"'{parentFolderId}' in parents");
                }

                request.Q = string.Join(" and ", queryParts);
                request.Fields = "files(id, name, createdTime, modifiedTime, webViewLink, parents)";
                request.PageSize = 1000;

                var result = await request.ExecuteAsync();
                LogInfo($"Retrieved {result.Files.Count} folders for user {userEmail} from parent folder: {parentFolderPath ?? "root"}");

                return result.Files;
            }
            catch (Exception ex)
            {
                LogError($"Error getting folders for user {userEmail}: {ex.Message}");
                throw;
            }
        }
        private string GetUserEmail()
        {
            // ✅ Step 1: Get DB Name from session
            string dbName = _httpContextAccessor.HttpContext?.Session.GetString("CustomerCode");

            if (string.IsNullOrEmpty(dbName))
                throw new Exception("CustomerCode is missing in session. Please log in again.");

            // ✅ Step 2: Get the connection string
            var connectionString = _configuration.GetConnectionString(dbName);

            // ✅ Step 3: Query the latest UserEmail
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT TOP 1 UserEmail FROM UserDriveTokens ORDER BY Id DESC";

            using var command = new SqlCommand(sql, connection);
            var result = command.ExecuteScalar();

            return result?.ToString();
        }
        // Add these methods to your existing GoogleDriveService class

        public async Task<Google.Apis.Drive.v3.Data.File> GetFileByIdAsync(int DocId, string userEmail)
        {
            if (DocId <= 0)
                throw new ArgumentException("Document Not Exist!");

            string fileId;
            userEmail = GetUserEmail();
            // Retrieve FileId from database
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                fileId = await connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT FileId FROM UserDriveItemsNumeric WHERE DocId = @DocId",
                    new { DocId });
            }

            if (string.IsNullOrEmpty(fileId))
            {
                throw new Exception($"No FileId found in database for DocId {DocId}");
            }
            
            var service = await GetDriveServiceAsync(userEmail);

            try
            {
                var request = service.Files.Get(fileId);
                request.Fields = "id, name, mimeType, size, createdTime, modifiedTime, webViewLink, webContentLink, parents, thumbnailLink, fileExtension, fullFileExtension, originalFilename";

                var file = await request.ExecuteAsync();

                LogInfo($"Retrieved file by ID: {fileId} for user {userEmail}");
                return file;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LogError($"File not found with ID: {fileId} for user {userEmail}");
                throw new Exception($"File with ID '{fileId}' not found.", ex);
            }
            catch (Exception ex)
            {
                LogError($"Error getting file by ID {fileId} for user {userEmail}: {ex.Message}");
                throw;
            }
        }


        public async Task<GoogleDriveFile> GetFolderByIdAsync(string folderId, string userEmail)
        {
            if (string.IsNullOrEmpty(folderId))
                throw new ArgumentException("Folder ID must be provided.", nameof(folderId));
            userEmail = GetUserEmail();
            var service = await GetDriveServiceAsync(userEmail);

            try
            {
                var request = service.Files.Get(folderId);
                request.Fields = "id, name, mimeType, createdTime, modifiedTime, webViewLink, parents";
                var folder = await request.ExecuteAsync();

                // Verify it's actually a folder
                if (folder.MimeType != "application/vnd.google-apps.folder")
                {
                    LogError($"ID {folderId} is not a folder for user {userEmail}");
                    throw new Exception($"The ID '{folderId}' does not belong to a folder.");
                }

                LogInfo($"Retrieved folder by ID: {folderId} for user {userEmail}");
                return folder;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LogError($"Folder not found with ID: {folderId} for user {userEmail}");
                throw new Exception($"Folder with ID '{folderId}' not found.", ex);
            }
            catch (Exception ex)
            {
                LogError($"Error getting folder by ID {folderId} for user {userEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task<object> GetStorageIndicatorAsync(string userEmail)
        {
            try
            {
                userEmail = GetUserEmail();
                var service = await GetDriveServiceAsync(userEmail);
                var aboutRequest = service.About.Get();
                aboutRequest.Fields = "storageQuota";
                var about = await aboutRequest.ExecuteAsync();
                var quota = about.StorageQuota;

                long limit = quota.Limit ?? 0;
                long used = quota.Usage ?? 0;
                double usagePercent = limit > 0 ? Math.Round((double)used / limit * 100, 2) : 0;
                long free = limit - used;

                // Choose a color or indicator level
                string indicator = usagePercent switch
                {
                    >= 90 => "🔴 Critical (90%+ used)",
                    >= 75 => "🟠 Warning (75%+ used)",
                    >= 50 => "🟡 Moderate (50%+ used)",
                    _ => "🟢 Healthy (<50% used)"
                };

                return new
                {
                    Status = "Success",
                    Used = FormatBytes(used),
                    Total = FormatBytes(limit),
                    Free = FormatBytes(free),
                    UsagePercent = usagePercent,
                    Indicator = indicator
                };
            }
            catch (Exception ex)
            {
                LogError($"Error getting storage info for {userEmail}: {ex.Message}");
                return new { Status = "Error", Message = ex.Message };
            }
        }

        // Helper method
        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n2} {suffixes[counter]}";
        }

    }
}