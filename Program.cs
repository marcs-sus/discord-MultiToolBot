using System;
using System.Reflection;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Utilities;
using DiscordBot.Modules;

namespace DiscordBot
{
    public class Program
    {
        private static DiscordSocketClient _client = new DiscordSocketClient();
        private static CommandService _commands = new CommandService();
        private static ServiceCollection _services = new ServiceCollection();
        private const string BOT_PREFIX = "rob"; // Bot prefix
        public const string COMMAND_PREFIX = "!"; // Command prefix

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
            _services.AddSingleton(_commands);
            var provider = _services.BuildServiceProvider();

            // Create a command context and register the commands
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            // Cancel if the message is null or if the author is a bot
            if (message == null || message.Author.IsBot) return;

            var context = new SocketCommandContext(_client, message);

            // Call the Hello and Help command when only mentioned
            string mention = $"<@{_client.CurrentUser.Id}>";
            if (message.Content.Trim() == mention)
            {
                await context.Channel.SendMessageAsync
                    ($"Hello **{context.User.Mention}**! I am your bot. Use `{COMMAND_PREFIX}help` to see what I can do!");
                return;
            }

            int argPosition = 0;

            // Check if the message is in a DM or a server channel
            if (context.IsPrivate)
            {
                // In DMs, respond to commands with just the COMMAND_PREFIX
                if (message.HasStringPrefix(COMMAND_PREFIX, ref argPosition))
                {
                    try
                    {
                        await _commands.ExecuteAsync(context, argPosition, null);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            else
            {
                // In server channels, respond to commands with BOT_PREFIX + COMMAND_PREFIX or mentions
                string fullPrefix = $"{BOT_PREFIX}{COMMAND_PREFIX}";
                if (message.HasStringPrefix(fullPrefix, ref argPosition) || message.HasMentionPrefix(_client.CurrentUser, ref argPosition))
                {
                    try
                    {
                        await _commands.ExecuteAsync(context, argPosition, null);
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
}
