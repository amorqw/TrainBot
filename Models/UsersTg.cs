namespace TrainBot.Models;

public class UsersTg
{
    public long TelegramId { get; set; }
    public string Username { get; set; }= string.Empty;
    public DateTime? RegistrationDate { get; set; }

    public List<ExercisesTg> ExercisesTg { get; set; } = new List<ExercisesTg>();

}