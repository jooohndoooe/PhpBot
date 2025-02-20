using PhpBot.Bot.BotService.CommandHandler;
using Telegram.Bot.Types.ReplyMarkups;

namespace PhpBot.Bot.BotService.TelegramHandler
{
    public class TelegramHandler : ITelegramHandler
    {
        public TelegramHandler(IEnumerable<ICommandHandler> handlers) 
        {
            _handlers = handlers;
        }
        public async Task<string[]> Handle(string[] input, CurrentUser user) 
        { 
            string command = input[0];
            var handler = _handlers.SingleOrDefault(h => h.Command == command);
            if (handler == null) 
            {
                return new string[] { "Invalid Input" };
            }

            if (await handler.CanPerform(user))
            {
                return await handler.Handle(input.Skip(1).ToArray(), user);
            }
            else 
            {
                return new string[] { "Unauthorized Access" };
            }
        }

        public async Task<ReplyKeyboardMarkup> GetMenu(CurrentUser user)
        {
            var buttons = new List<KeyboardButton>();

            foreach (var handler in _handlers) 
            {
                if (await handler.ToShow(user)) 
                {
                    buttons.Add(new KeyboardButton(handler.Title));
                }
            }

            return new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>(){  buttons.ToArray() })
            {
                ResizeKeyboard = true,
            };
        }

        private IEnumerable<ICommandHandler> _handlers;
    }
}
