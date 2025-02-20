using Telegram.Bot.Types.ReplyMarkups;

namespace PhpBot.Bot.BotService.TelegramHandler
{
    public interface ITelegramHandler
    {
        Task<string[]> Handle(string[] input, CurrentUser user);
        Task<ReplyKeyboardMarkup> GetMenu(CurrentUser user);
    }
}
