using Google.Apis.Drive.v3;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TracePca.Interface.Master
{
    public interface IGoogleDriveService
    {
        Task<DriveService> GetDriveServiceAsync(string userEmail);
        Task<string> GetOrCreateFolderAsync(string folderName, string parentId, string userEmail);
        Task<string> GetFolderIdByPathAsync(string folderPath, string userEmail);
        Task<object> UploadFileToFolderAsync(IFormFile file, string folderPath, string userEmail,int docid);
        Task<IEnumerable<GoogleDriveFile>> ListFilesAsync(string userEmail);
        Task<GoogleDriveFile> RenameFileAsync(string fileId, string newName, string userEmail);
        Task DeleteFileAsync(string fileId, string userEmail);
        Task ExchangeCodeAsync(string userEmail, string code);
        Task<int> CheckEmailExistsAsync(string gmail);

        // New methods for GetFiles and GetFolders
        Task<IEnumerable<GoogleDriveFile>> GetFilesAsync(string userEmail, string folderPath = null);
        Task<IEnumerable<GoogleDriveFile>> GetFoldersAsync(string userEmail, string parentFolderPath = null);

        Task<GoogleDriveFile> GetFileByIdAsync(int DocId, string userEmail);
        Task<GoogleDriveFile> GetFolderByIdAsync(string folderId, string userEmail);

        Task<object> GetStorageIndicatorAsync(string userEmail);

    }
}