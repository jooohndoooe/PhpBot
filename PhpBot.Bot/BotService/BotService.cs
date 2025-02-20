using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

namespace PhpBot.Bot.BotService
{
    public class BotService : IBotService
    {
        public BotService(BotServiceConfigurtion botServiceConfigurtion, HttpRequestHelper.IHttpRequestHelper httpHelper, TelegramHandler.ITelegramHandler telegramHandler)
        {
            _botClient = new TelegramBotClient(botServiceConfigurtion.TelegramSecret);
            _httpHelper = httpHelper;
            _telegramHandler = telegramHandler;
        }
        public async Task Start()
        {
            var me = await _botClient.GetMe();
            _botClient.StartReceiving(UpdateHandler, ErrorHandler);
            await Task.Delay(-1);
        }

        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;
                            var user = message.From;
                            var chat = message.Chat;

                            switch (message.Type)
                            {
                                case MessageType.Text:
                                    {
                                        var currentUser = await _httpHelper.SendRequest<string, CurrentUser>("/api/auth/getUser", HttpMethod.Post, user.Username);
                                        var outputs = await _telegramHandler.Handle(message.Text.Split('\n'), currentUser);

                                        currentUser = await _httpHelper.SendRequest<string, CurrentUser>("/api/auth/getUser", HttpMethod.Post, user.Username);
                                        var keyboard = await _telegramHandler.GetMenu(currentUser);

                                        foreach (var output in outputs)
                                        {
                                            await botClient.SendMessage(chat.Id, output, replyMarkup: keyboard);
                                        }
                                        return;
                                    }

                                default:
                                    {
                                        await botClient.SendMessage(
                                            chat.Id,
                                            "Use Only Text!");
                                        return;
                                    }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private readonly ITelegramBotClient _botClient;
        private readonly HttpRequestHelper.IHttpRequestHelper _httpHelper;
        private readonly TelegramHandler.ITelegramHandler _telegramHandler;
    }
}