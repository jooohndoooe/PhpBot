using FluentAssertions;
using Moq;
using NUnit.Framework;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Data.Auth;

namespace PhpBot.Api.Core.Test.Auth
{
    [TestFixture]
    public class AccessServiceTest
    {
        [Test]
        public async Task IsAuthorized_Guest()
        {
            var repo = new Mock<IAuthRepository>();

            IAccessService accessService = new AccessService(repo.Object);

            var authResult = await accessService.GetUserAuthorizationInfo(1);
            authResult.Should().BeNull();
        }

        [Test]
        public async Task IsAuthorized_User()
        {
            var repo = new Mock<IAuthRepository>();
            repo.Setup(r => r.GetAccessLevel(1)).ReturnsAsync(1);

            IAccessService accessService = new AccessService(repo.Object);

            var authResult = await accessService.GetUserAuthorizationInfo(1);
            authResult.UserId.Should().Be(1);
            authResult.AccessLevel.Should().Be(AuthResult.USER);
        }

        [Test]
        public async Task IsAuthorized_Admin()
        {
            var repo = new Mock<IAuthRepository>();
            repo.Setup(r => r.GetAccessLevel(1)).ReturnsAsync(2);

            IAccessService accessService = new AccessService(repo.Object);

            var authResult = await accessService.GetUserAuthorizationInfo(1);
            authResult.UserId.Should().Be(1);
            authResult.AccessLevel.Should().Be(AuthResult.ADMIN);
        }
    }
}
