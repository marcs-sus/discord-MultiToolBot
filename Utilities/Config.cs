using System;
using Discord;

namespace DiscordBot.Utilities
{
    /// <summary>
    /// This class is responsible for storing and retrieving configuration values.
    /// </summary>
    public static class Config
    {
        public static string CommandPrefix { get; set; } = "!";
        public static string BotName { get; set; } = "robot";

        /// <summary>
        /// Maps configuration values from a dictionary.
        /// </summary>
        /// <param name="configJson"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void MapConfig(Dictionary<string, string> configJson)
        {
            if (configJson == null) throw new ArgumentNullException(nameof(configJson));

            foreach (var kvp in configJson)
            {
                switch (kvp.Key)
                {
                    case "CommandPrefix":
                        CommandPrefix = kvp.Value;
                        break;
                    case "BotName":
                        BotName = kvp.Value;
                        break;
                    // Add more cases here for new config values
                    default:
                        Console.WriteLine($"Unrecognized config key: {kvp.Key}");
                        break;
                }
            }
        }
    }
}