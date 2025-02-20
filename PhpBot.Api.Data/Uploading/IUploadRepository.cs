namespace PhpBot.Api.Data.Uploading
{
    public interface IUploadRepository
    {
        Task<bool> AddUpload(
            string appName,
            string appBundle,
            string sftpHost,
            string sftpLogin,
            string sftpFilePath,
            int userId,
            DateTime uploadTime,
            string secretKey,
            string secretKeyParm
        );

        Task<List<Entities.Upload>> LastTenUploads();
    }
}