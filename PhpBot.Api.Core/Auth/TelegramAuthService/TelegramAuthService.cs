using Microsoft.Extensions.Logging;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Data.Auth;

namespace PhpBot.Api.Core.Auth.TelegramAuthService
{
    public class TelegramAuthService : ITelegramAuthService
    {
        public TelegramAuthService(IAuthRepository authRepository, IAccessService accessService, ILogger<TelegramAuthService> logger)
        {
            ArgumentNullException.ThrowIfNull(authRepository);
            ArgumentNullException.ThrowIfNull(accessService);
            ArgumentNullException.ThrowIfNull(logger);

            _authRepository = authRepository;
            _accessService = accessService;
            _logger = logger;
        }

        public async Task<bool> LoginWithTelegram(string login, string password, string telegramUsername)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(login);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);
            ArgumentException.ThrowIfNullOrWhiteSpace(telegramUsername);

            _logger.LogInformation("User {login} logged with telegram account {telegramUsername}", login, telegramUsername);

            var userId = await _authRepository.GetUserIdByTelegramUsername(telegramUsername);
            if (userId != null)
            {
                var authResult = await _accessService.GetUserAuthorizationInfo(userId.Value);
                return authResult != null;
            }

            var user = await _authRepository.FindUserByLogin(login);
            if (user == null)
            {
                _logger.LogWarning("User {0} is not found", login);
                return false;
            }

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                await _authRepository.SaveTelegramAccount(user.Id, telegramUsername);
                _logger.LogInformation(
                    "User {login} signed in successfully with telegram account {telegramUsername}",
                    login,
                    telegramUsername
                );
                return true;
            }

            _logger.LogWarning("User {0} has sign in error", login);
            return false;
        }

        public async Task<bool> LogoutTelegramAccount(string telegramUsername)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(telegramUsername);

            _logger.LogInformation("Telegram account {telegramUsername} has been logged out", telegramUsername);

            var success = await _authRepository.RemoveTelegramAccount(telegramUsername);
            return success;
        }

        private readonly IAuthRepository _authRepository;
        private readonly IAccessService _accessService;
        private readonly ILogger _logger;
    }
}