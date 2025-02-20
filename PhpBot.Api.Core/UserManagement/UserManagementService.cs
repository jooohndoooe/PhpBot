using Microsoft.Extensions.Logging;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Data.UserManagement;

namespace PhpBot.Api.Core.UserManagement
{
    public class UserManagementService : IUserManagementService
    {
        public UserManagementService(
            IUserManagementRepository userManagementRepository,
            IAccessService accessService,
            ILogger<UserManagementService> logger
        )
        {
            ArgumentNullException.ThrowIfNull(userManagementRepository);
            ArgumentNullException.ThrowIfNull(accessService);
            ArgumentNullException.ThrowIfNull(logger);

            _userManagementRepository = userManagementRepository;
            _accessService = accessService;
            _logger = logger;
        }

        public async Task<bool> ChangeAccessLevel(int userToChangeId, int accessLevel, int userId)
        {
            if (accessLevel <= 0 || accessLevel >= 3)
            {
                throw new ArgumentException("Invalid Access Level");
            }

            _logger.LogWarning(
                "Admin {adminId} requested to change permission level for user {userId} to {level}",
                userId,
                userToChangeId,
                accessLevel
            );

            var authResult = await _accessService.GetUserAuthorizationInfo(userId);
            if (authResult == null || authResult.AccessLevel < 2)
            {
                throw new UnauthorizedAccessException("Unauthorized Access");
            }

            if (userToChangeId == userId)
            {
                throw new ArgumentException("Cant change own access level");
            }

            var isChanged = await _userManagementRepository.ChangeAccessLevel(userToChangeId, accessLevel);
            if (isChanged)
            {
                _logger.LogWarning(
                    "Admin {adminId} successfully changed permission level for user {userId} to {level}",
                    userId,
                    userToChangeId,
                    accessLevel
                );
            }
            else
            {
                _logger.LogError("Error: changing permission level for user {userId} to {level}", userId, userToChangeId, accessLevel);
            }

            return isChanged;
        }

        private readonly IUserManagementRepository _userManagementRepository;
        private readonly IAccessService _accessService;
        private readonly ILogger _logger;
    }
}