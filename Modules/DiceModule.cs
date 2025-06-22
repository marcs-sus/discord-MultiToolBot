using System;
using System.Text;
using Discord;
using Discord.Commands;

namespace DiscordBot.Modules
{
    /// <summary>
    /// This module contains a command that rolls a dice with a specified number of sides and rolls.
    /// </summary>
    [Name("Dice")]
    [Summary("Roll dices with specified number of sides and number of rolls.")]
    public class DiceModule : ModuleBase<SocketCommandContext>
    {
        [Command("roll")]
        [Summary("Rolls a dice with the specified number of sides and rolls.")]
        public async Task RollAsync(int sides = 6, long rolls = 1)
        {
            // Validate the number of sides and rolls
            if (sides < 2 || sides > 100)
            {
                await ReplyAsync("Number of sides must be between 2 and 100.");
                return;
            }

            if (rolls < 1 || rolls > 50)
            {
                await ReplyAsync("Number of rolls must be between 1 and 50.");
                return;
            }

            // Define variables
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            int total = 0;

            // Create an EmbedBuilder to build the response message
            var embed = new EmbedBuilder()
                .WithTitle($"Rolling a {sides}-sided dice {rolls} time(s):")
                .WithColor(Color.Blue);

            // Roll the dice and calculate the total and average
            for (int i = 0; i < rolls; i++)
            {
                int roll = random.Next(1, sides + 1);
                total += roll;
                embed.AddField($"Roll {i + 1}", roll, inline: true);
            }

            if (rolls > 1)
            {
                float average = (float)total / rolls;
                embed.AddField("Average", $"{average:F2}", inline: false);
            }

            // Send the embed as a reply
            await ReplyAsync(embed: embed.Build());
        }
    }
}