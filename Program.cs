using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TrainBot;
using TrainBot.Data;
using TrainBot.Services;

using var cts = new CancellationTokenSource();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

var telegramBotToken = builder.Configuration["TelegramBotToken"];
var bot = new TelegramBotClient(telegramBotToken, cancellationToken: cts.Token);
var me = await bot.GetMeAsync(); 

builder.Services.AddScoped<ITelegramBotClient>(_ => bot);
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("ConnectionStringss"), 
        new MySqlServerVersion(new Version(9, 0, 1))));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExercisesService>();
builder.Services.AddScoped<Pdf>();

var serviceProvider = builder.Services.BuildServiceProvider();
var botHandler = new BotHandlers(bot, serviceProvider.GetRequiredService<UserService>(), 
    serviceProvider.GetRequiredService<ExercisesService>(), serviceProvider.GetRequiredService<Pdf>());

var app = builder.Build();
app.Run();