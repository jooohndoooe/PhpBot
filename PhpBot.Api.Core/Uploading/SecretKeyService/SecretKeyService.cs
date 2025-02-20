namespace PhpBot.Api.Core.Uploading.SecretKeyService
{
    public class SecretKeyService : ISecretKeyService
    {
        public async Task<SecretResult> GetSecret()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string secretKey = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
            string secretKeyParam = "key" + random.Next().ToString();

            return new SecretResult { Secret = secretKey, SecretKeyParam = secretKeyParam };
        }
    }
}
