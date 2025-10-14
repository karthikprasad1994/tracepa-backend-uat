using Google.Apis.Drive.v3;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace TracePca.Interface.Master
{

    public interface IGoogleDriveService
    {
        Task<DriveService> GetDriveServiceAsync(string userEmail);
        Task<string> GetOrCreateFolderAsync(string folderName, string parentId, string userEmail);
        Task<string> GetFolderIdByPathAsync(string folderPath, string userEmail);
        Task<object> UploadFileToFolderAsync(IFormFile file, string folderPath, string userEmail);
            Task<IEnumerable<GoogleDriveFile>> ListFilesAsync(string userEmail);
        Task<GoogleDriveFile> RenameFileAsync(string fileId, string newName, string userEmail);
        Task DeleteFileAsync(string fileId, string userEmail);



    }
}
