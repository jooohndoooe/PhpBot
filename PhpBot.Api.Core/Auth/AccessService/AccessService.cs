using PhpBot.Api.Data.Auth;

namespace PhpBot.Api.Core.Auth.AccessService
{
    public class AccessService : IAccessService
    {
        public AccessService(IAuthRepository authRepository)
        {
            ArgumentNullException.ThrowIfNull(authRepository);
            _authRepository = authRepository;
        }

        public async Task<AuthResult?> GetUserAuthorizationInfo(int userId)
        {
            var accessLevel = await _authRepository.GetAccessLevel(userId);

            if (accessLevel == null || accessLevel == 0)
            {
                return null;
            }

            var authResult = new AuthResult {UserId = userId, AccessLevel = accessLevel.Value};
            return authResult;
        }

        public async Task<CurrentUser> GetCurrentUser(string telegramUsername)
        {
            var userId = await _authRepository.GetUserIdByTelegramUsername(telegramUsername);

            return new CurrentUser {Id = userId ?? 0, IsAuthorized = userId != null, TelegramUsername = telegramUsername};
        }

        private readonly IAuthRepository _authRepository;
    }
}