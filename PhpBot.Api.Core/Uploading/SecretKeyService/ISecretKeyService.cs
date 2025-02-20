namespace PhpBot.Api.Core.Uploading.SecretKeyService
{
    public interface ISecretKeyService
    {
        public Task<SecretResult> GetSecret();
    }
}
