namespace TrainBot.Models;

public class UsersTg
{
    public Guid TelegramId { get; set; }
    public string Username { get; set; }
    public DateTime? RegistrationDate { get; set; }
    
    public List<ExercisesTg> ExercisesTg { get; set; }
    
}