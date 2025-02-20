using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PhpBot.Api.Data.Uploading
{
    public class UploadRepository : IUploadRepository
    {
        public UploadRepository(PhpBotDbContext db)
        {
            _db = db;
        }

        public async Task<bool> AddUpload(string appName, string appBundle, string sftpHost, string sftpLogin, string sftpFilePath, int userId, DateTime uploadTime, string secretKey, string secretKeyParam)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(appName);
            ArgumentException.ThrowIfNullOrWhiteSpace(appBundle);
            ArgumentException.ThrowIfNullOrWhiteSpace(sftpHost);
            ArgumentException.ThrowIfNullOrWhiteSpace(sftpFilePath);
            ArgumentException.ThrowIfNullOrWhiteSpace(sftpLogin);
            ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(secretKeyParam);

            await _db.Uploads.AddAsync(new Entities.Upload
            {
                UserId = userId,
                AppName = appName,
                AppBundle = appBundle,
                SFTPHost = sftpHost,
                SFTPLogin = sftpLogin,
                SFTPFilePath = sftpFilePath,
                UploadTime = uploadTime,
                SecretKey = secretKey,
                SecretKeyParam = secretKeyParam
            });
            return true;
        }

        public async Task<List<Entities.Upload>> LastTenUploads()
        {
            return await _db.Uploads.OrderByDescending(x => x.UploadTime).Take(10).ToListAsync();
        }

        private readonly PhpBotDbContext _db;
    }
}
