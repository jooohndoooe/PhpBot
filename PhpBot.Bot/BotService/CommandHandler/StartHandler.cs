namespace PhpBot.Bot.BotService.CommandHandler
{
    public class StartHandler : ICommandHandler
    {
        public string Command => "/start";
        public string Title => "/start";

        public async Task<bool> CanPerform(CurrentUser user)
        {
            return true;
        }

        public async Task<bool> ToShow(CurrentUser user)
        {
            return await CanPerform(user);
        }

        public async Task<string[]> Handle(string[] input, CurrentUser user)
        {
            if (user.IsAuthorized)
            {
                return new string[] { "Hi! What can I help you with today?" };
            }
            else 
            {
                return new string[] { "Please log in to use the bot!" };
            }
        }
    }
}
