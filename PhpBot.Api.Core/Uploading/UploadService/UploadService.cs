using System.Text;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Core.Uploading.SecretKeyService;
using PhpBot.Api.Data.Uploading;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PhpBot.Api.Core.SftpClient;

namespace PhpBot.Api.Core.Uploading.UploadService
{
    public class UploadService : IUploadService
    {
        public UploadService(
            IUploadRepository uploadRepository,
            IAccessService accessService,
            ISecretKeyService secretKeyService,
            ISftpClient sftpClient,
            ILogger<UploadService> logger
        )
        {
            ArgumentNullException.ThrowIfNull(uploadRepository);
            ArgumentNullException.ThrowIfNull(accessService);
            ArgumentNullException.ThrowIfNull(secretKeyService);
            ArgumentNullException.ThrowIfNull(sftpClient);
            ArgumentNullException.ThrowIfNull(logger);


            _uploadRepository = uploadRepository;
            _accessService = accessService;
            _secretKeyService = secretKeyService;
            _sftpClient = sftpClient;
            _logger = logger;
        }

        public async Task<List<Upload>> LastTenUploads(int userId)
        {
            _logger.LogInformation("User {id} requested info about last uploads", userId);

            var authResult = await _accessService.GetUserAuthorizationInfo(userId);
            if (authResult == null || authResult.AccessLevel < 2)
            {
                throw new UnauthorizedAccessException("Unauthorized Access");
            }

            var lastTenUploadsFromDatabase = await _uploadRepository.LastTenUploads();

            return lastTenUploadsFromDatabase.Select(
                                                  e => new Upload
                                                       {
                                                           UserId = e.UserId,
                                                           AppName = e.AppName,
                                                           AppBundle = e.AppBundle,
                                                           SFTPHost = e.SFTPHost,
                                                           SFTPLogin = e.SFTPLogin,
                                                           SFTPFilePath = e.SFTPFilePath,
                                                           UploadTime = e.UploadTime,
                                                           SecretKey = e.SecretKey,
                                                           SecretKeyParam = e.SecretKeyParam,
                                                       }
                                              )
                                             .ToList();
        }

        public async Task<UploadResult> Upload(
            string appName,
            string appBundle,
            string sftpHost,
            string sftpLogin,
            string sftpPassword,
            string sftpFilePath,
            int userId
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(appName);
            ArgumentException.ThrowIfNullOrWhiteSpace(appBundle);
            ArgumentException.ThrowIfNullOrWhiteSpace(sftpHost);
            ArgumentException.ThrowIfNullOrWhiteSpace(sftpLogin);
            ArgumentException.ThrowIfNullOrWhiteSpace(sftpPassword);
            ArgumentException.ThrowIfNullOrWhiteSpace(sftpFilePath);

            _logger.LogInformation(
                "User {id} started SFTP upload. Host={host}, login={login}, path={path}. App Info: name={appName} bundle={appBundle}",
                userId,
                sftpHost,
                sftpLogin,
                sftpFilePath,
                appName,
                appBundle
            );

            var authResult = await _accessService.GetUserAuthorizationInfo(userId);
            if (authResult == null)
            {
                throw new UnauthorizedAccessException("Unauthorized Access");
            }

            var validPattern = @"^[a-zA-Z0-9\s.]*$";
            if (!Regex.IsMatch(appName, validPattern) || !Regex.IsMatch(appBundle, validPattern))
            {
                _logger.LogError("App info has invalid symbols, rejected to prevent PHP code injection");
                throw new ArgumentException("Invalid input");
            }

            var secretResult = await _secretKeyService.GetSecret();
            var phpFile = GeneratePhpFile(appName, appBundle, secretResult.SecretKeyParam, secretResult.Secret);
            var uploadResult = await _sftpClient.Upload(
                                   new SftpConnectionParameters {Host = sftpHost, Login = sftpLogin, Password = sftpPassword},
                                   sftpFilePath,
                                   Encoding.UTF8.GetBytes(phpFile)
                               );

            if (!uploadResult.Success)
            {
                _logger.LogError("Error uploading data to SFTP: {error}", uploadResult.Error);
                return new UploadResult {ErrorMesage = $"Error uploading to SFTP: {uploadResult.Error}", Success = false};
            }

            var succsess = await _uploadRepository.AddUpload(
                               appName,
                               appBundle,
                               sftpHost,
                               sftpLogin,
                               sftpFilePath,
                               userId,
                               DateTime.Now,
                               secretResult.Secret,
                               secretResult.SecretKeyParam
                           );
            if (!succsess)
            {
                _logger.LogError("Error saving upload information to db");
                var uplodadResult = new UploadResult
                                    {
                                        Success = succsess,
                                        ErrorMesage = "Upload error",
                                        SecretKey = null,
                                        SecretKeyParam = null
                                    };
                return uplodadResult;
            }
            else
            {
                _logger.LogInformation("Upload completed successfully");
                
                var uplodadResult = new UploadResult
                                    {
                                        Success = succsess,
                                        ErrorMesage = "",
                                        SecretKey = secretResult.Secret,
                                        SecretKeyParam = secretResult.SecretKeyParam
                                    };
                return uplodadResult;
            }
        }

        private string GeneratePhpFile(string appName, string appBundle, string secretKey, string secret)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(appName);
            ArgumentException.ThrowIfNullOrWhiteSpace(appBundle);
            ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(secret);

            return $$"""
                     <?php

                     $appName = '{{appName}}';
                     $appBundle = '{{appBundle}}';

                     $secretKey = '{{secret}}';


                     if($secretKey == $_GET['{secretKey}']) {
                         echo 'Привет я приложение '. $appName .' моя ссылка на гугл плей https://play.google.com/store/apps/details?id='. $appBundle ;
                     }
                     """;
        }

        private readonly IUploadRepository _uploadRepository;
        private readonly IAccessService _accessService;
        private readonly ISecretKeyService _secretKeyService;
        private readonly ISftpClient _sftpClient;
        private readonly ILogger _logger;
    }
}