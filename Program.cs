using System.Collections.Immutable;
using Microsoft.AspNetCore.Builder;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrainBot.Data;
using TrainBot;
using TrainBot.DB;

/*using var cts = new CancellationTokenSource();
var config = Config.LoadConfig();
var bot = new TelegramBotClient(config.TelegramBotToken, cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
var db = new DataBase();
var botHandler = new BotHandlers(bot, db);*/


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();
builder.Services.AddScoped<ITelegramBotClient, TelegramBotClient>(cfg =>
{
    string? token = builder.Configuration["TelegramBotToken"] ?? throw new ArgumentNullException("Token is not specified");
    return new TelegramBotClient(token);
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("ConnectionStringss"), 
    new MySqlServerVersion(new Version(9, 0, 1))));

WebApplication app = builder.Build();
app.MapGet("/",  () => "Hello World!");
app.Run();
/*db.CheckConnect();
Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
Console.ReadLine();
cts.Cancel();*/