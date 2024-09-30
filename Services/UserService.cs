using Telegram.Bot.Types;
using TrainBot.Data;
using TrainBot.Models;

namespace TrainBot.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public bool UserExists(long userId)
    {
        return _context.UsersTg.Any(u => u.TelegramId ==  userId);
    }

    public void UserAdd(int userId, string username)
    {
        var user = new UsersTg
        {
            TelegramId = userId,
            Username = username,
            RegistrationDate = DateTime.Now
        };
        _context.UsersTg.Add(user);
        _context.SaveChanges();
    }
}