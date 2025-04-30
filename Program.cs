using System;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot
{
    public class Program
    {
        private static DiscordSocketClient _client;
        private static CommandService _commands;
        private static IServiceProvider _services;

        public static async Task Main()
        {
            try
            {
                _client = new DiscordSocketClient();
                _commands = new CommandService();

                _client.Log += Log;

                var token = File.ReadAllText("token.txt");

                await RegisterCommandsAsync();

                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message == null || message.Author.IsBot) return;

            int argPosition = 0;

            if (message.HasStringPrefix("!", ref argPosition) || message.HasMentionPrefix(_client.CurrentUser, ref argPosition))
            {
                try
                {
                    var result = await _commands.ExecuteAsync(context, argPosition, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}