using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TracePca.Interface.Master;

namespace TracePca.Service.Master
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private static readonly string[] Scopes = { DriveService.Scope.DriveFile, DriveService.Scope.Drive };
        private readonly string _tokenDirectory = "Tokens";
        private readonly string _credentialsPath;

        public GoogleDriveService()
        {
            _credentialsPath = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? "client_secret_desktop.json"
                : "client_secret.json";
        }

        public async Task<DriveService> GetDriveServiceAsync(string userEmail)
        {
            if (!File.Exists(_credentialsPath))
                throw new FileNotFoundException($"Credentials file not found: {_credentialsPath}");

            using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                userEmail,
                CancellationToken.None,
                new FileDataStore(_tokenDirectory, true)
            );

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive CRUD MultiUser"
            });
        }

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
            return folder.Id;
        }

        public async Task<string> GetFolderIdByPathAsync(string folderPath, string userEmail)
        {
            var service = await GetDriveServiceAsync(userEmail);
            string parentId = null;
            var folders = folderPath.Split('/', System.StringSplitOptions.RemoveEmptyEntries);

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

        public async Task<object> UploadFileToFolderAsync(IFormFile file, string folderPath, string userEmail)
        {
            if (file == null || file.Length == 0)
                return new { Status = "Error", Message = "No file uploaded." };

            var service = await GetDriveServiceAsync(userEmail);

            var folderId = await GetFolderIdByPathAsync(folderPath, userEmail);
            if (folderId == null)
            {
                return new
                {
                    Status = "Error",
                    Message = $"Folder path '{folderPath}' does not exist in Google Drive for {userEmail}."
                };
            }

            var fileMetadata = new GoogleDriveFile
            {
                Name = file.FileName,
                Parents = new[] { folderId }
            };

            using var stream = file.OpenReadStream();
            var request = service.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "id, name, parents";
            await request.UploadAsync();

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
            return await request.ExecuteAsync();
        }

        public async Task DeleteFileAsync(string fileId, string userEmail)
        {
            var service = await GetDriveServiceAsync(userEmail);
            await service.Files.Delete(fileId).ExecuteAsync();
        }
    }
}
