using PhpBot.Bot.Requests;
using PhpBot.Bot.Responses;

namespace PhpBot.Bot.BotService.CommandHandler
{
    public class ChangePermissionsHandler : ICommandHandler
    {
        public string Command => ":change";
        public string Title => "Change Permissions";

        public ChangePermissionsHandler(HttpRequestHelper.IHttpRequestHelper httpHelper)
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
            return false;
        }

        public async Task<string[]> Handle(string[] input, CurrentUser user)
        {
            var ChangeAccessLevelRequestContent = new ChangeAccessLevelRequest { UserToChangeId = Int32.Parse(input[0]), AccessLevel = Int32.Parse(input[1]) };
            try
            {
                await _httpHelper.SendRequest<ChangeAccessLevelRequest, string>("/api/userManagement/changeAccessLevel", HttpMethod.Post, user.TelegramUsername, ChangeAccessLevelRequestContent);
                return new string[] { $"Changed Access Level for user#{input[0]} to {input[1]}" };
            }
            catch (InvalidOperationException ex)
            {
                return new string[] { "Invalid Operation" };
            }
        }

        private readonly HttpRequestHelper.IHttpRequestHelper _httpHelper;
    }
}
