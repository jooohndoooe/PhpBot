namespace PhpBot.Bot.BotService.CommandHandler
{
    public class LoginHelpHandler : ICommandHandler
    {
        public string Command => "Login";
        public string Title => "Login";

        public async Task<bool> CanPerform(CurrentUser user)
        {
            return !user.IsAuthorized;
        }
        
        public async Task<bool> ToShow(CurrentUser user)
        {
            return await CanPerform(user);
        }

        public async Task<string[]> Handle(string[] input, CurrentUser user)
        {
            return new string [] { "Please provide your login informtion in th following format:\n" +
                                   "\n" +
                                   ":login\n" + 
                                   "Your Login\n" + 
                                   "Your Password" };
        }
    }
}
