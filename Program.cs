using System.IO;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string botToken = configuration["BotToken"];
        
        System.Console.ReadLine();
    }
}