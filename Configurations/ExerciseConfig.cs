using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainBot.Models;

namespace TrainBot.Configurations;

public class ExerciseConfig:IEntityTypeConfiguration<ExercisesTg>
{
    public void Configure(EntityTypeBuilder<ExercisesTg> builder)
    {
        builder.HasKey(e => e.TelegramId);
        builder.Property(e => e.Exercise)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.Weight)
            .IsRequired();
        builder.Property(e => e.Repetitions)
            .IsRequired();
        builder.Property(e => e.Date)
            .IsRequired(false);
    }
}