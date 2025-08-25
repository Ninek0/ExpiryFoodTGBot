using ExpiryFoodTGBot;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

using var cts = new CancellationTokenSource();

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory + "/Settings/")
    .AddJsonFile("appsettings.json")
    .Build();

//var section = config.GetSection("BotConfig");

//var tgBotConfig = config.Get<BotConfig>();

var botHandler = new BotHandler(
    new TelegramBotClient(config.GetSection("BotConfig").GetValue<string>("BotToken")));
Console.ReadLine();
cts.Cancel();