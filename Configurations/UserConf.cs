using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainBot.Models;

namespace TrainBot.Configurations;

public class UserConf: IEntityTypeConfiguration<UsersTg>
{
    public void Configure(EntityTypeBuilder<UsersTg> builder)
    {
        builder.HasKey(u => u.TelegramId);
        builder.HasMany(u => u.ExercisesTg)
            .WithOne(e => e.UsersTg)
            .HasForeignKey(e => e.TelegramId);
    }
}