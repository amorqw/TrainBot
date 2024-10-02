﻿using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.RegularExpressions;
using System;
namespace TrainBot.Services
{
    public class BotHandlers
    {
        private readonly TelegramBotClient _bot;
        private ReplyKeyboardMarkup _replyKeyboard;
        private readonly UserService _userService;
        private readonly ExercisesService _exercisesService;
        private readonly Pdf _pdf;

        public BotHandlers(TelegramBotClient bot, UserService userService, ExercisesService exercisesService, Pdf pdf)
        {
            _bot = bot;
            _userService = userService;
            _exercisesService = exercisesService;
            _pdf = pdf;
            bot.OnMessage += OnMessage;
            bot.OnUpdate += OnUpdate;
            bot.OnError += OnError;
        }

        private static readonly InlineKeyboardMarkup IlineKeyboarding = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("➕ Добавить упражнение", "Добавить упражнение"),
                InlineKeyboardButton.WithCallbackData("📥 Скачать .pdf файл", "Скачать .pdf")
            }
        });

        private static readonly InlineKeyboardMarkup BackButtonKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Назад", "Назад")
            }
        });

        private Dictionary<long, string> _userStates = new Dictionary<long, string>();
        private Dictionary<long, string> _usersExName= new Dictionary<long, string>();
        private Dictionary<long, float> _usersExWeight= new Dictionary<long, float>();
        private Dictionary<long, string> _usersExReps= new Dictionary<long, string>();

        private async Task OnMessage(Message msg, UpdateType type)
        {
            Console.WriteLine(
                $"Сообщение от пользователя {msg.From} (Id: {msg.MessageId} msg: {msg.Text}), id пользователя: {msg.From.Id}");
            if (!_userService.UserExists(msg.From.Id))
            {
                await _bot.SendTextMessageAsync(msg.Chat.Id, "Добро пожаловать в Тренировочного Бота!");
                _userService.UserAdd((int)msg.From.Id, msg.From.Username);
                Console.WriteLine($"Пользователь {msg.From.Username} (ID: {msg.From.Id}) добавлен в базу данных.");
            }

            if (msg.Text == null)
            {
                await _bot.SendTextMessageAsync(msg.Chat.Id, "Бот понимает только текст, увы!");
                return;
            }

            switch (msg.Text)
            {
                case "/start":
                    await _bot.SendPhotoAsync(msg.Chat.Id,
                        "https://avatars.dzeninfra.ru/get-zen_doc/1616946/pub_5e35338152d3287a8c81fdcf_5e355e32ebb18a27f5041990/scale_1200",
                        caption: $"Выбери действие", replyMarkup: IlineKeyboarding);
                    break;
                default:
                    if (!_userStates.ContainsKey(msg.Chat.Id))
                    {
                        await _bot.SendTextMessageAsync(msg.Chat.Id,
                            "Возможно вы ввели неизвестную команду, \nдля начала работы введите " + "/start");
                        return;
                    }

                    switch (_userStates[msg.Chat.Id])
                    {
                        case "input_exerciseName":
                            _usersExName[msg.From.Id] = msg.Text;
                            _userStates[msg.Chat.Id] = "input_exerciseWeight";
                            await _bot.SendTextMessageAsync(msg.Chat.Id, "Красава,Теперь введи вес");
                            /*var parts = msg.Text.Split(' ');
                            if (parts.Length == 3)
                            {
                                _exercisesService.AddExercise((int)msg.Chat.Id,(int)msg.From.Id, parts[0], float.Parse(parts[1]), int.Parse(parts[2]), DateTime.Now);
                                await _bot.SendPhotoAsync(msg.Chat.Id,
                                    "https://avatars.dzeninfra.ru/get-zen_doc/1711960/pub_5e88ce2901822a01b722c6a5_5e88dadb13cc2b78dcfaf0b1/scale_1200",
                                    caption:"Красава, не опозорился\nМожешь ввести новое упражнение, либо скачать .pdf файл", replyMarkup: IlineKeyboarding );
                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(msg.Chat.Id, "Некорректный формат. Используйте: <упражнение> <вес> <повторы>.");
                            }*/
                            break;
                        case "input_exerciseWeight":
                            /*await _bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId-1);*/
                            
                            if (float.TryParse(msg.Text, out float weight))
                            {
                                _usersExWeight[msg.Chat.Id] = weight;
                                _userStates[msg.Chat.Id] = "input_exerciseReps";
                                await _bot.SendTextMessageAsync(msg.Chat.Id, "Теперь введи повторения");
                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(msg.Chat.Id, "Некорректный ввод. Введите число");
                            }
                            break;
                        case "input_exerciseReps":
                            _usersExReps[msg.From.Id] = msg.Text;
                            /*await _bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId-1);*/
                                if(int.TryParse(_usersExReps[msg.From.Id], out int reps))
                                {
                                    _exercisesService.AddExercise((int)msg.Chat.Id, (int)msg.From.Id, _usersExName[msg.From.Id], _usersExWeight[msg.From.Id], reps, DateTime.Now);
                                    await _bot.SendPhotoAsync(msg.Chat.Id,
                                        "https://avatars.dzeninfra.ru/get-zen_doc/1711960/pub_5e88ce2901822a01b722c6a5_5e88dadb13cc2b78dcfaf0b1/scale_1200",
                                        caption:"Красава, не опозорился\nМожешь ввести новое упражнение, либо скачать .pdf файл", replyMarkup: IlineKeyboarding );
                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(msg.Chat.Id, "Некорректный ввод.Введите целое число");
                                return;
                            }
                            break;

                        default:
                            await _bot.SendTextMessageAsync(msg.Chat.Id,
                                "Возможно вы ввели неизвестную команду, \nдля начала работы введите " + "/start");
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

                switch (query.Data)
                {
                    case "Добавить упражнение":
                        await _bot.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);
                        await _bot.SendTextMessageAsync(query.Message.Chat.Id,
                            "Горилла, какое упражнение делал?",
                            replyMarkup: BackButtonKeyboard);
                        _userStates[query.Message.Chat.Id] = "input_exerciseName";
                        break;

                    case "Скачать .pdf":
                        var exercises = _exercisesService.GetUserExercises((int)query.From.Id);
                        if (exercises == null || !exercises.Any())
                        {
                            await _bot.SendTextMessageAsync(query.Message.Chat.Id,
                                "Введи хотя бы 1 упражнение пожалуйста");
                            return;
                        }
                        else
                        {
                            string fileName = $"{query.From.Id}.pdf";
                            _pdf.ManipulatePdf(fileName, exercises);

                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Pdf", fileName);
                            {
                                await using Stream stream = System.IO.File.OpenRead(filePath);
                                await _bot.SendDocumentAsync(query.Message.Chat.Id,
                                    document: InputFile.FromStream(stream, $"{query.Message.Chat.Id}.pdf"));
                            }
                            _pdf.DelPdf(filePath);
                        }

                        _userStates[query.Message.Chat.Id] = "/start";

                        break;

                    case "Назад":
                        await _bot.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);

                        await _bot.SendPhotoAsync(
                            query.Message.Chat.Id,
                            "https://avatars.dzeninfra.ru/get-zen_doc/1616946/pub_5e35338152d3287a8c81fdcf_5e355e32ebb18a27f5041990/scale_1200",
                            caption: $"{query.From.Username} Выбери действие:",
                            replyMarkup: IlineKeyboarding
                        );

                        _userStates[query.Message.Chat.Id] = "/start";
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