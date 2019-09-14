using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BDONotiBot.Code
{
    public class AppCommandHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _discordClient;
        private readonly CommandService _commands;

        public AppCommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commands)
        {
            _serviceProvider = services;
            _commands = commands;
            _discordClient = client;
        }

        public async Task InstallCommandsAsync()
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                _discordClient.MessageReceived += HandleCommandAsync;
                await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: serviceScope.ServiceProvider);
                
            }
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }
                

            int argPos = 0;

            if (!(message.HasCharPrefix('*', ref argPos) || message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos)) || message.Author.IsBot)
            {
                return;
            }
                

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var context = ActivatorUtilities.CreateInstance<SocketCommandContext>(serviceScope.ServiceProvider, _discordClient, message);

                var result = await _commands.ExecuteAsync(context: context, argPos: argPos, services: serviceScope.ServiceProvider);

                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
                    
            }
        }
    }
}
