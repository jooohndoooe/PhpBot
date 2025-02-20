using PhpBot.Bot.Requests;
using PhpBot.Bot.Responses;

namespace PhpBot.Bot.BotService.CommandHandler
{
    public class LoginHandler : ICommandHandler
    {
        public string Command => ":login";
        public string Title => "Login";

        public LoginHandler(HttpRequestHelper.IHttpRequestHelper httpHelper) 
        {
            _httpHelper = httpHelper;
        }

        public async Task<bool> CanPerform(CurrentUser user)
        {
            return !user.IsAuthorized;
        }

        public async Task<bool> ToShow(CurrentUser user)
        {
            return false;
        }

        public async Task<string[]> Handle(string[] input, CurrentUser user)
        {
            var loginRequestContent = new TelegramLoginRequest { Login = input[0], Password = input[1] };
            try
            {
                await _httpHelper.SendRequest<TelegramLoginRequest, string>("/api/auth/login", HttpMethod.Post, user.TelegramUsername, loginRequestContent);

                var accesslevelResponse = await _httpHelper.SendRequest<string, GetCurrentUserAccesslevelResponse>("/api/auth/getUserAccessLevel", HttpMethod.Post, user.TelegramUsername);


                return new string[] { "Logged in!", "Hi! What can I help you with today?" };
            }
            catch (InvalidOperationException ex)
            {
                return new string[] { "Invalid Operation" };
            }
        }

        private readonly HttpRequestHelper.IHttpRequestHelper _httpHelper;
    }
}
