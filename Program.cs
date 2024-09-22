using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrainBot;
using TrainBot.DB;

using var cts = new CancellationTokenSource();
var config = Config.LoadConfig();
var bot = new TelegramBotClient(config.TelegramBotToken, cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
var botHandler = new BotHandlers(bot);
var db= new DataBase();
db.CheckConnect();
Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel(); 



