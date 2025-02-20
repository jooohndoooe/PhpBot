using PhpBot.Bot.Responses;

namespace PhpBot.Bot.BotService.CommandHandler
{
    public class ChangePermissionsHelpHandler : ICommandHandler
    {
        public string Command => "Change Permissions";
        public string Title => "Change Permissions";

        public ChangePermissionsHelpHandler(HttpRequestHelper.IHttpRequestHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<bool> CanPerform(CurrentUser user)
        {
            GetCurrentUserAccesslevelResponse accesslevelResponse = await _httpHelper.SendRequest<string, GetCurrentUserAccesslevelResponse>("/api/auth/getUserAccessLevel", HttpMethod.Post, user.TelegramUsername);
            return accesslevelResponse.AccessLevel == 2;
        }

        public async Task<bool> ToShow(CurrentUser user)
        {
            return await CanPerform(user);
        }

        public async Task<string[]> Handle(string[] input, CurrentUser user)
        {
            return new string[] { "Please provide parameters in the following format:\n" +
                                  "\n" +
                                  ":change\n" +
                                  "UserId\n" +
                                  "New Access Level (1 for User, 2 for Admin)\n" };
        }

        private readonly HttpRequestHelper.IHttpRequestHelper _httpHelper;
    }
}
