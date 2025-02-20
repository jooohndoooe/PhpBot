using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhpBot.Bot.BotService;
using PhpBot.Bot.BotService.CommandHandler;
using PhpBot.Bot.BotService.TelegramHandler;
using PhpBot.Bot.HttpRequestHelper;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).AddEnvironmentVariables().Build();

var serviceCollection = new ServiceCollection();


var httpRequestHelperConfiguration = new HttpRequestHelperConfiguration
{
    ApiHost = configuration.GetValue<string>("ApiHost")
};
serviceCollection.AddSingleton(httpRequestHelperConfiguration);
serviceCollection.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
var botServiceConfiguration = new BotServiceConfigurtion
{
    TelegramSecret = configuration.GetValue<string>("TelegramApiToken")
};
serviceCollection.AddSingleton(botServiceConfiguration);
serviceCollection.AddSingleton<IBotService, BotService>();

serviceCollection.AddSingleton<ICommandHandler, ChangePermissionsHandler>();
serviceCollection.AddSingleton<ICommandHandler, ChangePermissionsHelpHandler>();
serviceCollection.AddSingleton<ICommandHandler, LastUploadsHandler>();
serviceCollection.AddSingleton<ICommandHandler, LoginHandler>();
serviceCollection.AddSingleton<ICommandHandler, LoginHelpHandler>();
serviceCollection.AddSingleton<ICommandHandler, LogoutHandler>();
serviceCollection.AddSingleton<ICommandHandler, StartHandler>();
serviceCollection.AddSingleton<ICommandHandler, UploadHandler>();
serviceCollection.AddSingleton<ICommandHandler, UploadHelpHandler>();

serviceCollection.AddSingleton<ITelegramHandler, TelegramHandler>();

var serviceProvider = serviceCollection.BuildServiceProvider();

await serviceProvider.GetRequiredService<IBotService>().Start();
