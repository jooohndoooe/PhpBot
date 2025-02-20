using FluentAssertions;
using NUnit.Framework;
using PhpBot.Api.Core.Uploading.SecretKeyService;

namespace PhpBot.Api.Core.Test.Upload
{
    [TestFixture]
    public class SecretKeyServiceTest
    {
        [Test]
        public async Task GetSecret_ReturnsData()
        {
            ISecretKeyService secretKeyService = new SecretKeyService();

            var secretResult = await secretKeyService.GetSecret();

            secretResult.Secret.Should().NotBeEmpty();
            secretResult.SecretKeyParam.Should().NotBeEmpty();
        }

        [Test]
        public async Task GetSecret_ReturnsDifferentData()
        {
            ISecretKeyService secretKeyService = new SecretKeyService();

            var secretResult1 = await secretKeyService.GetSecret();
            var secretResult2 = await secretKeyService.GetSecret();

            secretResult1.Secret.Should().NotBe(secretResult2.Secret);
            secretResult1.SecretKeyParam.Should().NotBe(secretResult2.SecretKeyParam);
        }
    }
}
