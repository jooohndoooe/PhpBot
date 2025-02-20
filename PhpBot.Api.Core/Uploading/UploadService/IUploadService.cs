namespace PhpBot.Api.Core.Uploading.UploadService
{
    public interface IUploadService
    {
        Task<UploadResult> Upload(
            string appName,
            string appBundle,
            string sftpHost,
            string sftpLogin,
            string sftpPassword,
            string sftpFilePath,
            int userId
        );

        Task<List<Upload>> LastTenUploads(int userId);
    }
}