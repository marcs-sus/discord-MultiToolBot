using System;
using Discord;
using Discord.Commands;

namespace DiscordBot.Modules
{
    /// <summary>
    /// This module contains a command that provides help information for the bot.
    /// </summary>
    [Name("Help")]
    [Summary("Provides help information for the bot.")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        public HelpModule(CommandService commands)
        {
            _commands = commands;
        }

        [Command("help")]
        [Summary("Lists all available modules and their summaries.")]
        public async Task HelpAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Available Commands")
                .WithColor(Color.Blue)
                .WithDescription("Here are the commands you can use:");

            foreach (var module in _commands.Modules)
            {
                string moduleName = module.Name;
                string moduleCommand = module.Commands.FirstOrDefault()?.Name ?? "No command available.";
                string moduleSummary = module.Summary ?? "No summary available.";

                embed.AddField(moduleName, $"`{Program.COMMAND_PREFIX}{moduleCommand}`\n{moduleSummary}");
            }

            await ReplyAsync(embed: embed.Build());
        }
    }
}