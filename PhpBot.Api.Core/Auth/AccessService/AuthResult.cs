namespace PhpBot.Api.Core.Auth.AccessService
{
    public class AuthResult
    {
        public const int USER = 1;
        public const int ADMIN = 2;

        public int UserId { get; set; }
        public int AccessLevel { get; set; }
    }
}
