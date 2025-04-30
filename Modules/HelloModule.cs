using System;
using Discord;
using Discord.Commands;

namespace DiscordBot.Modules
{
    public class HelloModule : ModuleBase<SocketCommandContext>
    {
        private string[] greetings = { "Hello!", "Hi there!",
                                    "Greetings!",
                                    "Salutations!" };
        private string[] responses = { "How can I assist you today?",
                                    "What brings you here?",
                                    "Need help with something?",
                                    "I'm here to help!" };

        [Command("hello")]
        [Summary("Replies with a greeting message.")]
        public async Task HelloAsync()
        {
            Random random = new Random();
            await ReplyAsync($"{greetings[random.Next(greetings.Length)]} {responses[random.Next(responses.Length)]}");
        }
    }
}