using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Core.UserManagement;
using PhpBot.Api.Data.UserManagement;

namespace PhpBot.Api.Core.Test.UserManagement
{
    [TestFixture]
    public class UserManagementServiceTest
    {
        [Test]
        public async Task ChangeAccessLevel_UserToAdmin()
        {
            var repo = new Mock<IUserManagementRepository>();
            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 2});
            var logger = new Mock<ILogger<UserManagementService>>();

            IUserManagementService userManagementService = new UserManagementService(repo.Object, accessService.Object, logger.Object);

            _ = await userManagementService.ChangeAccessLevel(2, 2, 1);
            repo.Verify(r => r.ChangeAccessLevel(2, 2));
        }

        [Test]
        public async Task ChangeAccessLevel_AdminToUser()
        {
            var repo = new Mock<IUserManagementRepository>();

            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 2});

            var logger = new Mock<ILogger<UserManagementService>>();

            IUserManagementService userManagementService = new UserManagementService(repo.Object, accessService.Object, logger.Object);

            _ = await userManagementService.ChangeAccessLevel(2, 1, 1);
            repo.Verify(r => r.ChangeAccessLevel(2, 1));
        }

        [Test]
        public async Task ChangeAccessLevel_InvalidAccessLevel()
        {
            var repo = new Mock<IUserManagementRepository>();

            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 2});

            var logger = new Mock<ILogger<UserManagementService>>();

            IUserManagementService userManagementService = new UserManagementService(repo.Object, accessService.Object, logger.Object);

            Func<Task> action = async () => { await userManagementService.ChangeAccessLevel(2, 3, 1); };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task ChangeAccessLevel_Unauthorized()
        {
            var repo = new Mock<IUserManagementRepository>();

            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 1});

            var logger = new Mock<ILogger<UserManagementService>>();

            IUserManagementService userManagementService = new UserManagementService(repo.Object, accessService.Object, logger.Object);

            Func<Task> action = async () => { await userManagementService.ChangeAccessLevel(2, 2, 1); };
            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Test]
        public async Task ChangeAccessLevel_ChangePersonalAccessLevel()
        {
            var repo = new Mock<IUserManagementRepository>();
            var accessService = new Mock<IAccessService>();
            accessService.Setup(s => s.GetUserAuthorizationInfo(1)).ReturnsAsync(new AuthResult {UserId = 1, AccessLevel = 2});

            var logger = new Mock<ILogger<UserManagementService>>();

            IUserManagementService userManagementService = new UserManagementService(repo.Object, accessService.Object, logger.Object);

            Func<Task> action = async () => { await userManagementService.ChangeAccessLevel(1, 1, 1); };
            await action.Should().ThrowAsync<ArgumentException>();
        }
    }
}