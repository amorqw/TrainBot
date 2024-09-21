using Telegram.Bot;

namespace TrainBot
{
    class Program
    {
        static void Main(string[] args)
        {
                var config = Config.LoadConfig();
                var botClient = new TelegramBotClient(config.TelegramBotToken);
                Console.WriteLine($"Tokent Bot:{config.TelegramBotToken}");


        }
    }
}
