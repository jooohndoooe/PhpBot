using Microsoft.AspNetCore.Mvc;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Core.Auth.TelegramAuthService;

namespace PhpBot.Api.Web.Controllers
{
    [ApiController, Route("api/auth")]

    public class AuthController : ControllerBase
    {
        public AuthController(ITelegramAuthService telegramAuthService, IAccessService accessService, CurrentUser currentUser)
        {
            _telegramAuthService = telegramAuthService;
            _accessService = accessService;
            _currentUser = currentUser;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> LoginWithTelegram([FromBody] TelegramLoginRequest telegramLoginRequest)
        {
            var succsess = await _telegramAuthService.LoginWithTelegram(telegramLoginRequest.Login, telegramLoginRequest.Password, _currentUser.TelegramUsername);

            if (succsess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            };
        }

        [HttpDelete, Route("logout")]
        public async Task<IActionResult> LogoutTelegramAccount()
        {
            var succsess = await _telegramAuthService.LogoutTelegramAccount(_currentUser.TelegramUsername);

            if (succsess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            };
        }

        [HttpPost, Route("getUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currentUser = await _accessService.GetCurrentUser(_currentUser.TelegramUsername);
            return Ok(currentUser);
        }

        [HttpPost, Route("getUserAccessLevel")]
        public async Task<IActionResult> GetCurrentUserAuthResult()
        {
            var currentUser = await _accessService.GetCurrentUser(_currentUser.TelegramUsername);
            var authResult = await _accessService.GetUserAuthorizationInfo(currentUser.Id);
            if (authResult == null)
            {
                return BadRequest();
            }
            return Ok(new GetCurrentUserAccessLevelResponse { AccessLevel = authResult.AccessLevel });
        }

        private ITelegramAuthService _telegramAuthService;
        private IAccessService _accessService;
        private CurrentUser _currentUser;
    }
}