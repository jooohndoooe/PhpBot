namespace PhpBot.Api.Data.UserManagement
{
    public interface IUserManagementRepository
    {
        Task<bool> ChangeAccessLevel(int userId, int accessLevel);
    }
}