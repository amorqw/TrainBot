using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TrainBot;
using TrainBot.Data;
using TrainBot.Services;

using var cts = new CancellationTokenSource();
var config = Config.LoadConfig();
var bot = new TelegramBotClient(config.TelegramBotToken, cancellationToken: cts.Token);
var me = await bot.GetMeAsync(); 

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

builder.Services.AddScoped<ITelegramBotClient>(_ => new TelegramBotClient(builder.Configuration["TelegramBotToken"]));
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("ConnectionStringss"), 
        new MySqlServerVersion(new Version(9, 0, 1))));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExercisesService>();

var serviceProvider = builder.Services.BuildServiceProvider();
var botHandler = new BotHandlers(bot, serviceProvider.GetRequiredService<UserService>(), 
    serviceProvider.GetRequiredService<ExercisesService>());

var app = builder.Build();
app.Run();