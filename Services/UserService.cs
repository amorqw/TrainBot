using Microsoft.EntityFrameworkCore;

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
    public async Task UserAdd(long userId, string username)
    {
        var existingUser = await _context.UsersTg.FirstOrDefaultAsync(u => u.TelegramId == userId);
        if (existingUser == null)
        {
            var newUser = new UsersTg
            {
                TelegramId = userId,
                Username = username,
                RegistrationDate = DateTime.Now
            };
            _context.UsersTg.Add(newUser);
            await _context.SaveChangesAsync();
        }
    }
}