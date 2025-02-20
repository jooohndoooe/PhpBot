using PhpBot.Bot.Requests;
using PhpBot.Bot.Responses;

namespace PhpBot.Bot.BotService.CommandHandler
{
    public class UploadHandler : ICommandHandler
    {
        public string Command => ":upload";
        public string Title => "Upload";

        public UploadHandler(HttpRequestHelper.IHttpRequestHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<bool> CanPerform(CurrentUser user)
        {
            return user.IsAuthorized;
        }
        public async Task<bool> ToShow(CurrentUser user)
        {
            return false;
        }

        public async Task<string[]> Handle(string[] input, CurrentUser user)
        {
            var UploadRequestContent = new UploadRequest { AppName = input[0], AppBundle = input[1], SFTPHost = input[2], SFTPLogin = input[3], SFTPPassword = input[4], SFTPFilePath = input[5] };
            try
            {
                var uploadResponse = await _httpHelper.SendRequest<UploadRequest, UploadResponse>("/api/upload/upload", HttpMethod.Post, user.TelegramUsername, UploadRequestContent);

                if (uploadResponse.Success)
                {
                    return new string[] { $"Upload succsessfull!\n" +
                                          "\n" +
                                          $"SecretKey: {uploadResponse.SecretKey}" +
                                          $"\nSecretKeyParam: {uploadResponse.SecretKeyParam}" };
                }
                else
                {
                    return new string[] { $"Upload unsuccsessfull!\n" +
                                          $"\n" +
                                          $"{uploadResponse.ErrorMesage}"};
                }
            }
            catch (InvalidOperationException ex)
            {
                return new string[] { "Invalid Operation" };
            }
        }

        private readonly HttpRequestHelper.IHttpRequestHelper _httpHelper;
    }
}
