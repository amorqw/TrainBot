using TrainBot.Data;
using TrainBot.Models;

namespace TrainBot.Services;

public class ExercisesService
{
    private readonly AppDbContext _context;

    public ExercisesService(AppDbContext context)
    {
        _context = context;
    }

    public void AddExercise(int Id,int userId, string exerciseName, float weight, int repetitions, DateTime date)
    {
        var exercise = new ExercisesTg
        {
            TelegramId = userId,
            Exercise = exerciseName,
            Weight = weight,
            Repetitions = repetitions,
            Date = date
        };
        _context.ExercisesTg.Add(exercise);
        _context.SaveChanges();
    }
}