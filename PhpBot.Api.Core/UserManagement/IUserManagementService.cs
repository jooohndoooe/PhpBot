namespace PhpBot.Api.Core.UserManagement
{
    public interface IUserManagementService
    {
        public Task<bool> ChangeAccessLevel(int userToChangeId, int accessLevel, int userId);
    }
}
