using System;
using System.Reflection;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities;
using DiscordBot.Modules;
using System.Text.Json;
using System.Net;

namespace DiscordBot
{
    public class Program
    {
        private static DiscordSocketClient _client = new DiscordSocketClient();
        private static CommandService _commands = new CommandService();
        private static ServiceCollection _serviceCollection = new ServiceCollection();

        // Rate limit configuration
        private static readonly Dictionary<ulong, DateTime> _lastCommandTime = new();
        private static readonly TimeSpan _rateLimitDelay = TimeSpan.FromSeconds(2);

        public static async Task Main()
        {
            try
            {
                var configJson = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("config.json"));
                Config.MapConfig(configJson!);

                _client.Log += Logger.Log;

                // Get the token from environment variable
                var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");

                // Await to register commands
                await RegisterCommandsAsync();

                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();

                await Task.Delay(Timeout.Infinite);
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
            _serviceCollection.AddSingleton(_commands);
            var provider = _serviceCollection.BuildServiceProvider();

            // Create a command context and register the commands
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            // Cancel if the message is null or if the author is a bot
            if (message == null || message.Author.IsBot) return;

            // Call the Hello and Help command when only mentioned
            string mention = $"<@{_client.CurrentUser.Id}>";
            if (message.Content.Trim() == mention)
            {
                await context.Channel.SendMessageAsync
                    ($"Hello **{context.User.Mention}**! I am your bot. Use `{Config.CommandPrefix}help` to see what I can do!");
                return;
            }

            int argPosition = 0;
            if (message.HasStringPrefix(Config.CommandPrefix, ref argPosition) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPosition))
            {
                // Rate limiting check
                if (_lastCommandTime.TryGetValue(context.User.Id, out var lastTime) &&
                    DateTime.UtcNow - lastTime < _rateLimitDelay)
                {
                    return;
                }

                // Update the last command time for the user
                _lastCommandTime[context.User.Id] = DateTime.UtcNow;

                // Execute the command
                IResult result = await _commands.ExecuteAsync(context, argPosition, null);
                if (!result.IsSuccess)
                {
                    string errorMsg = result.ErrorReason switch
                    {
                        "Unknown command." => $"Sorry, I don't recognize that command. Try `{Config.CommandPrefix}help` for a list of commands.",
                        _ => $"An error occurred: {result.ErrorReason}. Please try again or use `{Config.CommandPrefix}help`."
                    };
                    await context.Channel.SendMessageAsync(errorMsg);
                }
            }
        }
    }
}
