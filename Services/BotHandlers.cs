using System.Runtime.InteropServices.JavaScript;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TrainBot.Services
{
    public class BotHandlers
    {
        private readonly TelegramBotClient _bot;
        private readonly UserService _userService;
        private readonly ExercisesService _exercisesService;
        private readonly Pdf _pdf;

        private Dictionary<long, string> _userStates = new Dictionary<long, string>();
        private Dictionary<long, string> _userNames = new Dictionary<long, string>();

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

        private async Task OnMessage(Message msg, UpdateType type)
        {
            Console.WriteLine($"Сообщение от пользователя {msg.From} (Id: {msg.MessageId}, msg: {msg.Text}), id пользователя: {msg.From?.Id}");

            if (!_userService.UserExists(msg.From.Id))
            {
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
                    await _bot.SendTextMessageAsync(msg.Chat.Id, "Добро пожаловать в Тренировочного Бота! Давай познакомимся, как тебя зовут?");
                    _userStates[msg.Chat.Id] = "start";
                    break;

                default:
                    if (!_userStates.ContainsKey(msg.Chat.Id))
                    {
                        await _bot.SendTextMessageAsync(msg.Chat.Id, "Возможно вы ввели неизвестную команду, \nдля начала работы введите /start");
                        return;
                    }

                    await HandleUserState(msg);
                    break;
            }
        }

        private async Task HandleUserState(Message msg)
        {
            switch (_userStates[msg.Chat.Id])
            {
                case "start":
                    await HandleStartState(msg);
                    break;
                case "input_exercise":
                    await HandleInputExerciseState(msg);
                    break;
                case "selection_function":
                    // Implement functionality if needed
                    break;
                default:
                    await _bot.SendTextMessageAsync(msg.Chat.Id, "Возможно вы ввели неизвестную команду, \nдля начала работы введите /start");
                    break;
            }
        }

        private async Task HandleStartState(Message msg)
        {
            if (string.IsNullOrWhiteSpace(msg.Text))
            {
                await _bot.SendTextMessageAsync(msg.Chat.Id, "Вы ввели пустую строку, повторите пожалуйста ввод.");
                return;
            }

            if (!_userNames.ContainsKey(msg.Chat.Id))
            {
                _userNames[msg.Chat.Id] = msg.Text;
                _userStates[msg.Chat.Id] = "selection_function";

                await _bot.SendPhotoAsync(msg.Chat.Id, 
                    "https://avatars.dzeninfra.ru/get-zen_doc/1616946/pub_5e35338152d3287a8c81fdcf_5e355e32ebb18a27f5041990/scale_1200",
                    caption: $"Приятно познакомиться, {msg.Text}. Выбери действие:", 
                    replyMarkup: IlineKeyboarding);
            }
        }

        private async Task HandleInputExerciseState(Message msg)
        {
            var parts = msg.Text?.Split(' ');

            if (parts?.Length == 3)
            {
                _exercisesService.AddExercise((int)msg.Chat.Id, (int)msg.From.Id, parts[0], float.Parse(parts[1]), int.Parse(parts[2]), DateTime.Now);
                await _bot.SendPhotoAsync(msg.Chat.Id,
                    "https://avatars.dzeninfra.ru/get-zen_doc/1711960/pub_5e88ce2901822a01b722c6a5_5e88dadb13cc2b78dcfaf0b1/scale_1200",
                    caption: "Красава, не опозорился! Можешь ввести новое упражнение или скачать .pdf файл.", 
                    replyMarkup: IlineKeyboarding);
            }
            else
            {
                await _bot.SendTextMessageAsync(msg.Chat.Id, "Некорректный формат. Используйте: <упражнение> <вес> <повторы>.");
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
                            "Горилла, какое упражнение делал? С каким весом? Сколько раз?\nВведите в формате <Упражнение> <Число вес> <Число повторения>",
                            replyMarkup: BackButtonKeyboard);
                        _userStates[query.Message.Chat.Id] = "input_exercise";
                        break;
                    case "Скачать .pdf":
                        _pdf.ManipulatePdf($"{query.From.Username}.pdf");
                        
                        break;
                    case "Назад":
                        await _bot.DeleteMessageAsync(query.Message.Chat.Id, query.Message.MessageId);
                        
                        await _bot.SendPhotoAsync(
                            query.Message.Chat.Id,
                            "https://avatars.dzeninfra.ru/get-zen_doc/1616946/pub_5e35338152d3287a8c81fdcf_5e355e32ebb18a27f5041990/scale_1200",
                            caption:  $"{query.From.Username} Выбери действие:",
                            replyMarkup: IlineKeyboarding
                        );

                        _userStates[query.Message.Chat.Id] = "start";
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
