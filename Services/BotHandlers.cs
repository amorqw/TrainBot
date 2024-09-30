using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using TrainBot.Services;


namespace TrainBot
{
    public class BotHandlers
    {
        private readonly TelegramBotClient _bot;
        private ReplyKeyboardMarkup _replyKeyboard;
        private readonly UserService _userService;
        private readonly ExercisesService _exercisesService;
        

        public BotHandlers(TelegramBotClient bot, UserService userService, ExercisesService exercisesService)
        {
            
            _bot = bot;
            _userService = userService;
            _exercisesService = exercisesService;
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
            Console.WriteLine($"Сообщение от пользователя {msg.From} (Id: {msg.MessageId} msg: {msg.Text}), id пользователя: {msg.From.Id}");
            if (!_userService.UserExists(msg.From.Id))
            {
                // Если пользователя нет, добавляем его в таблицу `users`
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
                                    "Вы ввели пустую строку, повторите пожалуйста ввод и не оставляйте пустую строку ^_^");
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
                            var parts = msg.Text.Split(' ');
                            if (parts.Length == 3)
                            {
                                _exercisesService.AddExercise((int)msg.Chat.Id,(int)msg.From.Id, parts[0], float.Parse(parts[1]), int.Parse(parts[2]), DateTime.Now);
                                await _bot.SendPhotoAsync(msg.Chat.Id,
                                    "https://avatars.dzeninfra.ru/get-zen_doc/1711960/pub_5e88ce2901822a01b722c6a5_5e88dadb13cc2b78dcfaf0b1/scale_1200",
                                    caption:"Красава, не опозорился\nМожешь ввести новое упражнение, либо скачать .pdf файл", replyMarkup: _InlineKeyboard );
                            }
                            else
                            {
                                await _bot.SendTextMessageAsync(msg.Chat.Id, "Некорректный формат. Используйте: <упражнение> <вес> <повторы>.");
                            }
                            
                            break;
                        case "selection_pdf":
                            break;
                        default:
                            await _bot.SendTextMessageAsync(msg.Chat.Id, "Возможно вы ввели неизвестную команду, \nдля начала работы введите " +"/start");
                            break;
                    }
                    break;
            }
        }
        //To Do добавить кнопку назад, реализовать скачку pdf
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
                        await _bot.SendTextMessageAsync(query.Message.Chat.Id,"Горилла, какое упражнение делал? С каким весом? Сколько раз?\n" +
                                                                              "Введите в формате <Упражнение>  <Число вес>  <Число повторения>\nчерез пробел короче");
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