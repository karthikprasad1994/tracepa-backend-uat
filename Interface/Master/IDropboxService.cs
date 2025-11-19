namespace TracePA.Interfaces
{
    public interface IDropboxService
    {
        Task<string> GetAuthorizationUrlAsync();
        Task<string> ProcessCallbackAsync(string code);
        Task<string> UploadFileAsync(IFormFile file, string accessToken, string folderPath);
        Task<List<string>> ListFilesAsync(string accessToken, string folderPath);
        Task<Stream> DownloadFileAsync(string accessToken, string filePath);
    }
}
