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

namespace DiscordBot
{
    public class Program
    {
        private static DiscordSocketClient _client = new DiscordSocketClient();
        private static CommandService _commands = new CommandService();
        private static ServiceCollection _services = new ServiceCollection();

        public static async Task Main()
        {
            try
            {
                var configJson = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("config.json"));
                Config.MapConfig(configJson!);

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
            _services.AddSingleton(_commands);
            var provider = _services.BuildServiceProvider();

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
