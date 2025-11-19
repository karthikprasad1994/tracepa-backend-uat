using Dropbox.Api;
using Dropbox.Api.Files;
using TracePA.Interfaces;

namespace TracePA.Services
{
    public class DropboxService : IDropboxService
    {
        private readonly IConfiguration _config;

        public DropboxService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GetAuthorizationUrlAsync()
        {
            string appKey = _config["Dropbox:AppKey"];
            string redirectUri = _config["Dropbox:RedirectUri"];

            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Code, appKey, new Uri(redirectUri));
            return authorizeUri.ToString();
        }

        public async Task<string> ProcessCallbackAsync(string code)
        {
            string appKey = _config["Dropbox:AppKey"];
            string appSecret = _config["Dropbox:AppSecret"];
            string redirectUri = _config["Dropbox:RedirectUri"];

            var response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
      code, appKey, appSecret, redirectUri);


            return response.AccessToken;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string accessToken, string folderPath)
        {
            using (var dbx = new DropboxClient(accessToken))
            using (var mem = new MemoryStream())
            {
                await file.CopyToAsync(mem);
                mem.Position = 0;

                var uploadResult = await dbx.Files.UploadAsync(
                    $"{folderPath}/{file.FileName}",
                    WriteMode.Overwrite.Instance,
                    body: mem);

                return uploadResult.Id;
            }
        }

        public async Task<List<string>> ListFilesAsync(string accessToken, string folderPath)
        {
            using (var dbx = new DropboxClient(accessToken))
            {
                var list = await dbx.Files.ListFolderAsync(folderPath);
                return list.Entries.Where(i => i.IsFile).Select(i => i.Name).ToList();
            }
        }

        public async Task<Stream> DownloadFileAsync(string accessToken, string filePath)
        {
            using (var dbx = new DropboxClient(accessToken))
            {
                var response = await dbx.Files.DownloadAsync(filePath);
                return await response.GetContentAsStreamAsync();
            }
        }
    }
}
