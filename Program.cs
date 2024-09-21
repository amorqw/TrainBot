using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace TrainBot
{
    
    class Program
    {
        private static TelegramBotClient bot;
        static void Main(string[] args)
        {
            var config = Config.LoadConfig();
            Console.WriteLine($"Tokent Bot:{config.TelegramBotToken}");
            Console.WriteLine("proverka");
            Console.ReadLine();
            using var cts = new CancellationTokenSource();
            var bot = new TelegramBotClient(config.TelegramBotToken, cancellationToken: cts.Token);
            var me = bot.GetMeAsync().Result;
            bot.OnError += OnError;
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;

            cts.Cancel();
        }

        async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception); // just dump the exception to the console
        }

// method that handle messages received by the bot:
        async Task OnMessage(Message msg, UpdateType type)
        {
            if (msg.Text == "/start")
            {
                await bot.SendTextMessageAsync(msg.Chat, "Welcome! Pick one direction",
                    replyMarkup: new InlineKeyboardMarkup().AddButtons("Left", "Right"));
            }
        }

// method that handle other types of updates received by the bot:
        async Task OnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
            {
                await bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}");
                await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
            }
        }
    }
}
