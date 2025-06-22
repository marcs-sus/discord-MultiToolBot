using System;
using Discord;
using Discord.Commands;
using DiscordBot.Utilities;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Modules
{
    /// <summary>
    /// This module provides a command to fetch and display random dad's jokes.
    /// </summary>
    [Name("Joke")]
    [Summary("Displays random dad's jokes.")]
    public class JokeModule : ModuleBase<SocketCommandContext>
    {
        [Command("joke")]
        [Summary("Tells a random dad's joke.")]
        public async Task JokeAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "DiscordDadJokeBot (your.email@example.com)");

                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync("https://icanhazdadjoke.com/");
                        response.EnsureSuccessStatusCode();

                        string json = await response.Content.ReadAsStringAsync();
                        JObject jokeObject = JObject.Parse(json);
                        string joke = jokeObject["joke"]?.ToString() ?? "No joke found.";

                        await ReplyAsync($"*{joke}*");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching joke: {ex.Message}");
                        await ReplyAsync("Sorry, I couldn't fetch a joke at the moment. Please try again later.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "JokeModule", $"HTTP error: {ex.Message}"));
                await ReplyAsync("I'm having trouble connecting to the joke service. Please try again later.");
            }
            catch (Exception ex)
            {
                await Logger.Log(new LogMessage(LogSeverity.Error, "JokeModule", $"Unexpected error: {ex.Message}"));
                await ReplyAsync("Something went wrong. Please try again.");
            }
        }
    }
}