using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;

namespace DiscordBot.Modules
{
    /// <summary>
    /// This module contains a command that searches for images using DuckDuckGo and returns a random result.
    /// </summary>
    [Name("Image Search")]
    [Summary("This module contains a command that searches for images using DuckDuckGo and returns a random result.")]
    public class ImageSearchModule : ModuleBase<SocketCommandContext>
    {
        [Command("img")]
        [Summary("Searches for an image and returns a random result.")]
        public async Task ImageSearchAsync([Remainder] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                await ReplyAsync("Please provide a search term.");
                return;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                    httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                    httpClient.DefaultRequestHeaders.Add("Referer", "https://duckduckgo.com/");

                    string encodedSearchTerm = Uri.EscapeDataString(searchTerm);
                    string searchPageHtml = await httpClient.GetStringAsync($"https://duckduckgo.com/?q={encodedSearchTerm}");

                    Match vqdTokenMatch = Regex.Match(searchPageHtml, @"vqd=([\d-]+)&");
                    if (!vqdTokenMatch.Success)
                        throw new Exception("Failed to extract vqd token from DuckDuckGo page.");
                    string vqdToken = vqdTokenMatch.Groups[1].Value;

                    string searchRequestUrl = $"https://duckduckgo.com/i.js?q={encodedSearchTerm}&vqd={vqdToken}&o=json";
                    HttpResponseMessage searchResponse = await httpClient.GetAsync(searchRequestUrl);
                    searchResponse.EnsureSuccessStatusCode();

                    var searchResults = JsonSerializer.Deserialize<DuckDuckGoImageSearchResponse>
                        (await searchResponse.Content.ReadAsStringAsync());

                    if (searchResults != null && searchResults.results?.Count > 0)
                    {
                        Random random = new Random();
                        int randomIndex = random.Next(searchResults.results.Count);
                        string? randomImage = searchResults.results[randomIndex].image;

                        if (string.IsNullOrEmpty(randomImage))
                        {
                            await ReplyAsync($"No image found for '{searchTerm}'.");
                            return;
                        }

                        var embed = new EmbedBuilder()
                            .WithTitle($"Image Search Results for: {searchTerm}")
                            .WithImageUrl(randomImage)
                            .Build();
                        await ReplyAsync(embed: embed);
                    }
                    else
                    {
                        if (searchResults == null)
                        {
                            await ReplyAsync($"No results found for '{searchTerm}'.");
                        }

                        await ReplyAsync($"No results found for '{searchTerm}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ImageSearchAsync: {ex.Message} - {ex.StackTrace}");
                await ReplyAsync($"An error occurred while searching for images");
                return;
            }
        }

        public class DuckDuckGoImageSearchResponse
        {
            public string? query { get; set; }
            public List<ImageResult>? results { get; set; }
        }

        public class ImageResult
        {
            public string? image { get; set; }
        }
    }
}