using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PhpBot.Api.Core.Auth.TelegramAuthService;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Data.Auth;

namespace PhpBot.Api.Core.Test.Auth
{
    [TestFixture]
    public class TelegramAuthServiceTest
    {
        [Test]
        public async Task LoginWithTelegram_RealCredentials()
        {
            var repo = new Mock<IAuthRepository>();
            repo.Setup(r => r.FindUserByLogin("test"))
                .ReturnsAsync(
                     new Data.Entities.User
                     {
                         Id = 1,
                         Login = "test",
                         PasswordHash = "$2a$12$.UltGa.N7BOHF3UQTYmU5eUw//SGj3w3.BcR6yh1EFCwPpfQYRwzm",
                         AccessLevel = 1
                     }
                 );

            var accessService = new Mock<IAccessService>();
            var logger = new Mock<ILogger<TelegramAuthService>>();

            ITelegramAuthService telegramAuthService = new TelegramAuthService(repo.Object, accessService.Object, logger.Object);

            var loginResult = await telegramAuthService.LoginWithTelegram("test", "test", "test");
            loginResult.Should().BeTrue();
        }

        [Test]
        public async Task LoginWithTelegram_FakeCredentials()
        {
            var repo = new Mock<IAuthRepository>();
            var accessService = new Mock<IAccessService>();
            var logger = new Mock<ILogger<TelegramAuthService>>();

            var telegramAuthService = new TelegramAuthService(repo.Object, accessService.Object, logger.Object);

            var loginResult = await telegramAuthService.LoginWithTelegram("test", "test", "test");
            loginResult.Should().BeFalse();
        }

        [Test]
        public async Task LogoutTelegramAccount_Logout()
        {
            var repo = new Mock<IAuthRepository>();
            var accessService = new Mock<IAccessService>();
            var logger = new Mock<ILogger<TelegramAuthService>>();

            ITelegramAuthService telegramAuthService = new TelegramAuthService(repo.Object, accessService.Object, logger.Object);

            _ = await telegramAuthService.LogoutTelegramAccount("user");
            repo.Verify(r => r.RemoveTelegramAccount("user"));
        }
    }
}