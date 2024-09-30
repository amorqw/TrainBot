using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrainBot.Configurations;
using TrainBot.Models;
namespace TrainBot.Data;


public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<UsersTg> UsersTg { get; set; }
    public DbSet<ExercisesTg> ExercisesTg { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExerciseConfig());
        modelBuilder.ApplyConfiguration(new UserConf());
        base.OnModelCreating(modelBuilder);
    }
}