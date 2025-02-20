using PhpBot.Bot.Responses;
using Telegram.Bot.Types;

namespace PhpBot.Bot.BotService.CommandHandler
{
    internal class LastUploadsHandler : ICommandHandler
    {
        public string Command => "Last Uploads";
        public string Title => "Last Uploads";

        public LastUploadsHandler(HttpRequestHelper.IHttpRequestHelper httpHelper)
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
            try
            {
                var lastUploads = await _httpHelper.SendRequest<string, LastUploadsResponse>("/api/upload/lastUploads", HttpMethod.Get, user.TelegramUsername);

                string lastUplodsString = "";
                foreach (var upload in lastUploads.Uploads)
                {
                    lastUplodsString += $"UserId: {upload.UserId}\n" + 
                                        $"AppName: {upload.AppName}\n" + 
                                        $"AppBundle: {upload.AppBundle}\n" + 
                                        $"SFTPHost: {upload.SFTPHost} \n" +
                                        $"SFTPLogin: {upload.SFTPLogin}\n" + 
                                        $"SFTPFilePath: {upload.SFTPFilePath} \n" + 
                                        $"UploadTime: {upload.UploadTime}\n" + 
                                        "\n";
                }
                return new string[] { lastUplodsString };
            }
            catch (InvalidOperationException ex)
            {
                return new string[] { "Invalid Operation" };
            }
        }

        private readonly HttpRequestHelper.IHttpRequestHelper _httpHelper;
    }
}
