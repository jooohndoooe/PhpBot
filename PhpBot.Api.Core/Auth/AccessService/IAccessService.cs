namespace PhpBot.Api.Core.Auth.AccessService
{
    public interface IAccessService
    {
        Task<AuthResult?> GetUserAuthorizationInfo(int userId);
        
        Task<CurrentUser> GetCurrentUser(string telegramUsername); 
    }
}
