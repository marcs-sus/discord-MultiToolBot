using System;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities;

namespace DiscordBot
{
    public class Program
    {
        private static DiscordSocketClient _client = new DiscordSocketClient();
        private static CommandService _commands = new CommandService();
        private const string PREFIX = "!"; // Command prefix

        public static async Task Main()
        {
            try
            {
                _client.Log += Logger.Log;

                // Get the token from file
                var token = File.ReadAllText("token.txt");

                // Await to register commands
                await RegisterCommandsAsync();

                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static async Task RegisterCommandsAsync()
        {
            // Create a command context and register the commands
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message == null || message.Author.IsBot) return;

            int argPosition = 0;

            // Check if the message has a command prefix or mentions the bot
            if (message.HasStringPrefix(PREFIX, ref argPosition) || message.HasMentionPrefix(_client.CurrentUser, ref argPosition))
            {
                try
                {
                    // Execute the command with the context and arguments
                    IResult result = await _commands.ExecuteAsync(context, argPosition, null);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
