using Microsoft.EntityFrameworkCore;
using PhpBot.Api.Data.Entities;
using System.Collections.Immutable;

namespace PhpBot.Api.Data.Auth
{
    public class AuthRepository : IAuthRepository
    {
        public AuthRepository(PhpBotDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);

            _db = db;
        }

        public async Task<User?> FindUserByLogin(string login)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(login);

            return await _db.Users.SingleOrDefaultAsync(u => u.Login == login);
        }

        public async Task<int?> GetAccessLevel(int userId)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
            return user?.AccessLevel;
        }

        public async Task<int?> GetUserIdByTelegramUsername(string telegramUsername)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(telegramUsername);

            var telegramUser = await _db.TelegramUsers.SingleOrDefaultAsync(t => t.TelegramUsername == telegramUsername);
            return telegramUser?.UserId;
        }

        public async Task<bool> RemoveTelegramAccount(string telegramUsername)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(telegramUsername);

            var telegramUser = await _db.TelegramUsers.SingleOrDefaultAsync(t => t.TelegramUsername == telegramUsername);
            if (telegramUser == null)
                return false;

            _db.TelegramUsers.Remove(telegramUser);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task SaveTelegramAccount(int userId, string telegramUsername)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(telegramUsername);

            await _db.TelegramUsers.AddAsync(new TelegramUser {TelegramUsername = telegramUsername, UserId = userId});
            await _db.SaveChangesAsync();
        }

        private readonly PhpBotDbContext _db;
    }
}