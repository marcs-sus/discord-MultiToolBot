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
    [Summary("This module contains a command that rolls a dice with a specified number of sides and rolls.")]
    public class DiceModule : ModuleBase<SocketCommandContext>
    {
        [Command("roll")]
        [Summary("Rolls a dice with the specified number of sides and rolls.")]
        public async Task RollAsync(int sides = 6, int rolls = 1)
        {
            // Validate the number of sides and rolls
            if (sides < 2 || rolls < 1)
            {
                await ReplyAsync("Please provide a valid number of sides (>= 2) and rolls (>= 1).");
                return;
            }

            // Define variables
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            int total = 0;
            float avarage = 0;

            // Use StringBuilder to build the response message
            stringBuilder.AppendLine($"Rolling a **{sides}**-sided dice **{rolls}** time(s):");
            stringBuilder.AppendLine($"**Results**:");

            // Roll the dice and calculate the total and average
            for (int i = 0; i < rolls; i++)
            {
                int roll = random.Next(1, sides + 1);
                total += roll;
                stringBuilder.AppendLine($"Roll {i + 1} -> **{roll}**");
            }
            if (rolls > 1)
            {
                avarage = (float)total / rolls;
                stringBuilder.AppendLine($"Average: **{avarage:F2}**");
            }


            // Append the total to the message and reply
            await ReplyAsync(stringBuilder.ToString());
        }
    }
}