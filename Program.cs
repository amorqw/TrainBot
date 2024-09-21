using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace TrainBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Config.LoadConfig();
            var сlient = new TelegramBotClient(config.TelegramBotToken);
            сlient.StartReceiving(Update, Error);//тема будет обрабатывать обновы и ошибки
            Console.WriteLine($"Tokent Bot:{config.TelegramBotToken}");
            Console.WriteLine("proverka");
            Console.ReadLine();

        }

        async private static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token) 
        {
            var message = update.Message;
            if (message != null):
            {
                if (message.Text.ToLower().Contains("прив"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Привет!");
                    return;
                }
            }
        }

        async private static Task Error(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            
        }
    }
}
