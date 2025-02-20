namespace PhpBot.Api.Core.Auth.TelegramAuthService
{
    public interface ITelegramAuthService
    {
        Task<bool> LoginWithTelegram(string login, string password, string telegramUsername);
        Task<bool> LogoutTelegramAccount(string telegramUsername);
    }
}
