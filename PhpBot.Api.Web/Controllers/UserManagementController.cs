using Microsoft.AspNetCore.Mvc;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Core.UserManagement;

namespace PhpBot.Api.Web.Controllers
{
    [ApiController, Route("api/userManagement")]
    public class UserManagementController : ControllerBase
    {
        public UserManagementController(IUserManagementService userManagementService, CurrentUser currentUser)
        {
            _userManagementService = userManagementService;
            _currentUser = currentUser;
        }

        [HttpPost, Route("changeAccessLevel")]
        public async Task<IActionResult> ChangeAccessLevel([FromBody] ChangeAccessLevelRequest changeAccessLevelRequest)
        {
            if (!_currentUser.IsAuthorized)
            {
                return Unauthorized();
            }

            await _userManagementService.ChangeAccessLevel(changeAccessLevelRequest.UserToChangeId, changeAccessLevelRequest.AccessLevel, _currentUser.Id);
            return Ok();
        }

        private IUserManagementService _userManagementService;
        private CurrentUser _currentUser;
    }
}