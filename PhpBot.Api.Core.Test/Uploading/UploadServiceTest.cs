using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Core.SftpClient;
using PhpBot.Api.Core.Uploading.SecretKeyService;
using PhpBot.Api.Core.Uploading.UploadService;
using PhpBot.Api.Data.Uploading;

namespace PhpBot.Api.Core.Test.Upload
{
    [TestFixture]
    public class UploadServiceTest
    {
        [Test]
        public async Task Upload_GuestInput()
        {
            var repo = new Mock<IUploadRepository>();
            var accessService = new Mock<IAccessService>();
            var secretKeyService = new Mock<ISecretKeyService>();
            secretKeyService.Setup(s => s.GetSecret()).ReturnsAsync(new SecretResult {Secret = "test", SecretKeyParam = "test"});

            var sftpClient = new Mock<ISftpClient>();
            var logger = new Mock<ILogger<UploadService>>();

            IUploadService uploadService = new UploadService(
                repo.Object,
                accessService.Object,
                secretKeyService.Object,
                sftpClient.Object,
                logger.Object
            );

            var action = async () =>
                         {
                             await uploadService.Upload(
                                 "testApp",
                                 "testAppBundle",
                                 "testSftpHost",
                                 "testLogin",
                                 "testPwd",
                                 "/root/user/123.php",
                                 1
                             );
                         };
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task Upload_UserInput()
        {
            var repo = new Mock<IUploadRepository>();
            repo.Setup(
                     r => r.AddUpload(
                         "test",
                         "test",
                         "test",
                         "test",
                         "test",
                         1,
                         It.IsAny<DateTime>(),
                         "test",
                         "test"
                     )
                 )
                .ReturnsAsync(true);
            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 1});
            var secretKeyService = new Mock<ISecretKeyService>();
            secretKeyService.Setup(s => s.GetSecret()).ReturnsAsync(new SecretResult {Secret = "test", SecretKeyParam = "test"});

            var sftpClient = new Mock<ISftpClient>();
            sftpClient.Setup(c => c.Upload(It.IsAny<SftpConnectionParameters>(), It.IsAny<string>(), It.IsAny<byte[]>()))
                      .ReturnsAsync(new SftpUploadResult {Success = true});

            var logger = new Mock<ILogger<UploadService>>();

            IUploadService uploadService = new UploadService(
                repo.Object,
                accessService.Object,
                secretKeyService.Object,
                sftpClient.Object,
                logger.Object
            );

            var uploadResult = await uploadService.Upload(
                                   "test",
                                   "test",
                                   "test",
                                   "test",
                                   "test",
                                   "test",
                                   1
                               );
            uploadResult.Should().NotBeNull();
            uploadResult.Success.Should().BeTrue();
            uploadResult.ErrorMesage.Should().BeEmpty();
            uploadResult.SecretKey.Should().NotBeNullOrEmpty();
            uploadResult.SecretKeyParam.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Upload_AdminInput()
        {
            var appName = "com.test.app";
            var bundle = "com.test.bundle";
            var host = "sftp.test.com";
            var login = "sftplogin";
            var password = "sftppassword";
            var filePath = "/host/usr/index.php";

            var repo = new Mock<IUploadRepository>();
            repo.Setup(
                     r => r.AddUpload(
                         appName,
                         bundle,
                         host,
                         login,
                         filePath,
                         1,
                         It.IsAny<DateTime>(),
                         It.IsAny<string>(),
                         It.IsAny<string>()
                     )
                 )
                .ReturnsAsync(true);

            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 2});

            var secretKeyService = new Mock<ISecretKeyService>();
            secretKeyService.Setup(s => s.GetSecret()).ReturnsAsync(new SecretResult {Secret = "test", SecretKeyParam = "test"});

            var sftpClient = new Mock<ISftpClient>();
            sftpClient.Setup(c => c.Upload(It.IsAny<SftpConnectionParameters>(), It.IsAny<string>(), It.IsAny<byte[]>()))
                      .ReturnsAsync(new SftpUploadResult {Success = true});

            var logger = new Mock<ILogger<UploadService>>();

            IUploadService uploadService = new UploadService(
                repo.Object,
                accessService.Object,
                secretKeyService.Object,
                sftpClient.Object,
                logger.Object
            );

            var uploadResult = await uploadService.Upload(
                                   appName,
                                   bundle,
                                   host,
                                   login,
                                   password,
                                   filePath,
                                   1
                               );
            uploadResult.Should().NotBeNull();
            uploadResult.Success.Should().BeTrue();
            uploadResult.ErrorMesage.Should().BeEmpty();
            uploadResult.SecretKey.Should().NotBeEmpty();
            uploadResult.SecretKeyParam.Should().NotBeEmpty();

            sftpClient.Verify(
                c => c.Upload(
                    It.Is<SftpConnectionParameters>(v => v != null && v.Host == host && v.Login == login && v.Password == password),
                    It.Is<string>(filePath, StringComparer.Ordinal),
                    It.Is<byte[]>(b => b != null && b.Length > 1)
                )
            );
        }

        [Test]
        public async Task Upload_MaliciousInput()
        {
            var repo = new Mock<IUploadRepository>();
            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 2, AccessLevel = 2});

            var secretKeyService = new Mock<ISecretKeyService>();

            var sftpClient = new Mock<ISftpClient>();
            var logger = new Mock<ILogger<UploadService>>();

            IUploadService uploadService = new UploadService(
                repo.Object,
                accessService.Object,
                secretKeyService.Object,
                sftpClient.Object,
                logger.Object
            );

            var action = async () =>
                         {
                             await uploadService.Upload(
                                 "<?php $appName = '[AppName]';",
                                 "test",
                                 "test",
                                 "test",
                                 "test",
                                 "test",
                                 1
                             );
                         };


            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task LastTenUploads_AdminInput()
        {
            var repo = new Mock<IUploadRepository>();
            repo.Setup(r => r.LastTenUploads())
                .ReturnsAsync(
                     Enumerable.Range(1, 10)
                               .Select(
                                    i => new Data.Entities.Upload
                                         {
                                             Id = i,
                                             UserId = 1,
                                             AppName = "test",
                                             AppBundle = "test",
                                             SFTPHost = "test",
                                             SFTPLogin = "test",
                                             SFTPFilePath = "test",
                                             UploadTime = new DateTime(2002, 1, 1)
                                         }
                                )
                               .ToList()
                 );

            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 2});

            var secretKeyService = new Mock<ISecretKeyService>();
            var sftpClient = new Mock<ISftpClient>();
            var logger = new Mock<ILogger<UploadService>>();

            IUploadService uploadService = new UploadService(
                repo.Object,
                accessService.Object,
                secretKeyService.Object,
                sftpClient.Object,
                logger.Object
            );

            var lastTenUploads = await uploadService.LastTenUploads(1);
            lastTenUploads.Should().NotBeNull();
            lastTenUploads.Count().Should().BeLessThanOrEqualTo(10);
            foreach (var upload in lastTenUploads)
            {
                upload.UserId.Should().Be(1);
                upload.AppName.Should().NotBeNullOrEmpty();
                upload.AppBundle.Should().NotBeNullOrEmpty();
                upload.SFTPHost.Should().NotBeNullOrEmpty();
                upload.SFTPLogin.Should().NotBeNullOrEmpty();
                upload.SFTPFilePath.Should().NotBeNullOrEmpty();
                upload.UploadTime.Should().NotBe(null);
            }
        }

        [Test]
        public async Task LastTenUploads_UserInput()
        {
            var repo = new Mock<IUploadRepository>();

            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 1});

            var secretKeyService = new Mock<ISecretKeyService>();
            var sftpClient = new Mock<ISftpClient>();
            var logger = new Mock<ILogger<UploadService>>();

            IUploadService uploadService = new UploadService(
                repo.Object,
                accessService.Object,
                secretKeyService.Object,
                sftpClient.Object,
                logger.Object
            );

            var action = async () => { await uploadService.LastTenUploads(1); };
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task LastTenUploads_GuestInput()
        {
            var repo = new Mock<IUploadRepository>();
            var accessService = new Mock<IAccessService>();
            var secretKeyService = new Mock<ISecretKeyService>();
            var sftpService = new Mock<ISftpClient>();
            var logger = new Mock<ILogger<UploadService>>();

            IUploadService uploadService = new UploadService(
                repo.Object,
                accessService.Object,
                secretKeyService.Object,
                sftpService.Object,
                logger.Object
            );

            var action = async () => { await uploadService.LastTenUploads(1); };
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}