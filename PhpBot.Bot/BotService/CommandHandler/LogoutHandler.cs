namespace PhpBot.Bot.BotService.CommandHandler
{
    internal class LogoutHandler : ICommandHandler
    {
        public string Command => "Logout";
        public string Title => "Logout";

        public LogoutHandler(HttpRequestHelper.IHttpRequestHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<bool> CanPerform(CurrentUser user)
        {
            return user.IsAuthorized;
        }

        public async Task<bool> ToShow(CurrentUser user)
        {
            return await CanPerform(user);
        }

        public async Task<string[]> Handle(string[] input, CurrentUser user)
        {
            try
            {
                await _httpHelper.SendRequest<string, string>("/api/auth/logout", HttpMethod.Delete, user.TelegramUsername);
                return new string[] { "Logged out!" };
            }
            catch (InvalidOperationException ex)
            {
                return new string[] { "Invalid Operation" };
            }
        }

        private readonly HttpRequestHelper.IHttpRequestHelper _httpHelper;
    }
}
