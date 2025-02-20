namespace PhpBot.Bot.BotService.CommandHandler
{
    public interface ICommandHandler
    {
        string Command { get; }
        string Title { get; }
        Task<bool> CanPerform(CurrentUser user);
        Task<bool> ToShow(CurrentUser user);
        Task<string[]> Handle(string[] input, CurrentUser user);
    }
}
