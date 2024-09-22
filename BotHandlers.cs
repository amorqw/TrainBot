using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System;

namespace TrainBot
{
    public class BotHandlers
    {
        private readonly TelegramBotClient _bot;
        private ReplyKeyboardMarkup _replyKeyboard;

        public BotHandlers(TelegramBotClient bot)
        {
            _bot = bot;
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;
            bot.OnError += OnError;
            _replyKeyboard = new ReplyKeyboardMarkup(
                new List<KeyboardButton[]>()
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Добавить упражнение"),
                        new KeyboardButton("Скачать .pdf файл")
                    }
                })
            {
                ResizeKeyboard = true,
            };
        }
        private Dictionary<long,string> _userStates = new Dictionary<long, string>();
        private Dictionary<long,string> _userName = new Dictionary<long, string>();

        
        
        private async Task OnMessage(Message msg, UpdateType type)
        {
            Console.WriteLine($"Сообщение от пользователя {msg.From} (Id: {msg.MessageId} msg: {msg.Text})");

            if (msg.Text == null)
            {
                await _bot.SendTextMessageAsync(msg.Chat.Id, "Бот понимает только текст, увы!");
                return;
            }
            switch (msg.Text)
            {
                case "/start":
                    await _bot.SendTextMessageAsync(msg.Chat.Id, "Добро пожаловать в Тренировочного Бота!");
                    await _bot.SendTextMessageAsync(msg.Chat.Id, "Давай познакомимся, как тебя зовут?");
                    _userStates[msg.Chat.Id] = "start";
                    break;
                default:
                    if (!_userStates.ContainsKey(msg.Chat.Id))
                    {
                        
                        await _bot.SendTextMessageAsync(msg.Chat.Id, "Возможно вы ввели неизвестную команду, \nдля начала работы введите " +"/start");
                        
                        return;
                    }
                    switch (_userStates[msg.Chat.Id])
                    {
                        case "start":
                            if(string.IsNullOrWhiteSpace(msg.Text))
                                await _bot.SendTextMessageAsync(msg.Chat.Id,
                                    "Вы ввели пустую строку, повторите пожалуйста ввод и не оставляйте пустсую строку ^_^");
                            else
                            {
                                _userName[msg.Chat.Id] = msg.Text;
                                await _bot.SendTextMessageAsync(msg.Chat.Id, $"Приятно познакомиться, {msg.Text}!");
                                _userStates[msg.Chat.Id] = "selection_function";
                                await _bot.SendTextMessageAsync(msg.Chat.Id, $"{_userName[msg.Chat.Id]},выбери функцию", replyMarkup: _replyKeyboard);

                            }
                            break;
                        case "selection_function":
                            
                            break;
                    }
                    break;
            }
        }

        private async Task OnUpdate(Update update)
        {
            if (update.CallbackQuery != null)
            {
                var query = update.CallbackQuery;
                await _bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}");
                await _bot.SendTextMessageAsync(query.Message.Chat.Id, 
                    $"User {query.From.Username}, {query.From.Id} clicked on {query.Data}");
            }
        }

        private Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }
        
    }
    
}