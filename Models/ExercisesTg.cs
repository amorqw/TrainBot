using Telegram.Bot.Types;

namespace TrainBot.Models;

public class ExercisesTg
{
    public  int Id { get; set; }
    public int TelegramId { get; set; }
    public string Exercise { get; set; }=string.Empty;
    public float Weight { get; set; }
    public int Repetitions { get; set; }
    public DateTime? Date { get; set; }
    public string? Category { get; set; }
    
    public UsersTg UsersTg { get; set; }
}