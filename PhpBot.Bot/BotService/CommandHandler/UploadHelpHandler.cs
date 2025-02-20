namespace PhpBot.Bot.BotService.CommandHandler
{
    public class UploadHelpHandler : ICommandHandler
    {
        public string Command => "Upload";

        public string Title => "Upload";

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
            return new string[] { "Please provide upload info in the following format:\n" +
                                  "\n" +
                                  ":upload\n" +
                                  "AppName\n" +
                                  "AppBundle\n" +
                                  "SFTP Host\n" +
                                  "SFTP Login\n" +
                                  "SFTP Password\n" +
                                  "SFTP File Path"};
        }
    }
}
