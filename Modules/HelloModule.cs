using System;
using Discord;
using Discord.Commands;

namespace DiscordBot.Modules
{
    /// <summary>
    /// This module contains a command that replies with a greeting message.
    /// </summary>
    [Name("Hello")]
    [Summary("Replies users with a greetings messages.")]
    public class HelloModule : ModuleBase<SocketCommandContext>
    {
        private string[] greetings = { "Hello!", "Hi there!",
                                    "Greetings!",
                                    "Salutations!" }; // Strongly typed array of greetings
        private string[] responses = { "How can I assist you today?",
                                    "What brings you here?",
                                    "Need help with something?",
                                    "I'm here to help!" }; // Strongly typed array of responses

        [Command("hello")]
        [Summary("Replies with a greeting message.")]
        public async Task HelloAsync()
        {
            Random random = new Random();

            // Reply with a random greeting and response
            await ReplyAsync($"***{greetings[random.Next(greetings.Length)]} {responses[random.Next(responses.Length)]}***");
        }
    }
}