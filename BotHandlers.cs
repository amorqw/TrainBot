using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System;
using Microsoft.VisualBasic;
using Telegram.Bot.Requests;

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
            
        }

        private InlineKeyboardMarkup _InlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("➕ Добавить упражнение", "Добавить упражнение"),
                InlineKeyboardButton.WithCallbackData("📥 Скачать .pdf файл", "Скачать .pdf"),
            }
        });
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
                                _userStates[msg.Chat.Id] = "selection_function";
                                await _bot.SendPhotoAsync(msg.Chat.Id,
                                    "https://avatars.dzeninfra.ru/get-zen_doc/1616946/pub_5e35338152d3287a8c81fdcf_5e355e32ebb18a27f5041990/scale_1200",
                                    caption: $"Приятно познакомиться,{msg.Text}\nВыбери действие", replyMarkup: _InlineKeyboard);
                                _userStates[msg.Chat.Id] = "star221t";

                            }
                            break;
                        case "selection_function":
                            break;
                        case "input_exercise":
                        //to do Добавлять введенный текст в БД
                        case "selection_pdf":
                            break;
                        default:
                            await _bot.SendTextMessageAsync(msg.Chat.Id, "Возможно вы ввели неизвестную команду, \nдля начала работы введите " +"/start");
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
                await _bot.AnswerCallbackQueryAsync(query.Id, $"Вы выбрали: {query.Data}");
                await _bot.SendTextMessageAsync(query.Message.Chat.Id, 
                    $"User {query.From.Username}, {query.From.Id} clicked on {query.Data}");
                switch (query.Data)
                {
                    case "Добавить упражнение":
                        _userStates[query.Message.Chat.Id] = "input_exercise";
                        
                        break;
                    case "Скачать .pdf":
                        break;
                }
            }

            
        }

        private Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }
        
    }
    
}