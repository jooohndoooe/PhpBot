using Microsoft.EntityFrameworkCore;

namespace PhpBot.Api.Data.UserManagement
{
    public class UserManagementRepository : IUserManagementRepository
    {
        public UserManagementRepository(PhpBotDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);

            _db = db;
        }

        public async Task<bool> ChangeAccessLevel(int userId, int accessLevel)
        {
            var user = await _db.Users.SingleAsync(u => u.Id == userId);
            user.AccessLevel = accessLevel;
            await _db.SaveChangesAsync();

            return true;
        }

        private readonly PhpBotDbContext _db;
    }
}