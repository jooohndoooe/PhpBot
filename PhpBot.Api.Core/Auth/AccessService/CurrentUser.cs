namespace PhpBot.Api.Core.Auth.AccessService
{
    public class CurrentUser
    {
        public int Id { get; set; }
        public bool IsAuthorized { get; set; }
        public string TelegramUsername { get; set; }
    }
}
