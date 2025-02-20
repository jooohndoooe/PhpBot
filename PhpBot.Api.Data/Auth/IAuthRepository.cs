using PhpBot.Api.Data.Entities;

namespace PhpBot.Api.Data.Auth
{
    public interface IAuthRepository
    {
        Task<int?> GetAccessLevel(int userId);
        Task<bool> RemoveTelegramAccount(string telegramUsername);
        Task<User?> FindUserByLogin(string login);
        Task<int?> GetUserIdByTelegramUsername(string telegramUsername);
        
        Task SaveTelegramAccount(int userId, string telegramUsername);
    }
}