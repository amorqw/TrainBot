﻿using Newtonsoft.Json;

namespace TrainBot
{
    public class Config
    {
        public string TelegramBotToken { get; set; }

        public static Config LoadConfig()
        {
            var json = File.ReadAllText("..\\..\\..\\appsettings.json");
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}