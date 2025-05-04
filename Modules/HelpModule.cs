using System;
using Discord;
using Discord.Commands;
using DiscordBot.Utilities;

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
                .WithTitle("📚 Bot Command Help")
                .WithColor(Color.Blue)
                .WithDescription($"Use `{Config.CommandPrefix}<command>` to run a command. Mention me (@{Config.BotName}) instead of the prefix if preferred.\nBelow are all available commands:");

            // Iterate through all modules and their commands
            foreach (var module in _commands.Modules)
            {
                var commands = module.Commands
                    .Select(cmd => $"`{Config.CommandPrefix}{cmd.Name}`: {cmd.Summary ?? "No description available."}")
                    .ToList();

                // Add field on embed for each module with its commands
                if (commands.Any())
                {
                    string moduleDescription = $"{module.Summary ?? "No description available."}\n{string.Join("\n", commands)}";
                    embed.AddField(module.Name, moduleDescription, inline: false);
                }
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("help")]
        [Summary("Provides detailed help information for a specific command.")]
        public async Task HelpAsync(string commandName)
        {
            // Check if the command name is valid
            var command = _commands.Commands
                .FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));

            // Return if command is not found
            if (command == null)
            {
                await ReplyAsync($"Command `{commandName}` not found.");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle($"Command: {command.Name}")
                .WithColor(Color.Green)
                .WithDescription(command.Summary ?? "No description available.")
                .AddField("Usage", $"`{Config.CommandPrefix}{command.Name}{(command.Parameters.Any() ? " " + string.Join(" ", command.Parameters.Select(p => $"<{p.Name}>")) : "")}`")
                .AddField("Module", command.Module.Name);

            await ReplyAsync(embed: embed.Build());
        }
    }
}