using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BDONotiBot.Code
{
    public class BotApp : BackgroundService
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly AppCommandHandler _commandHandler;
        private readonly RespReminder _respReminder;
        
        public BotApp(DiscordSocketClient client, AppCommandHandler commandHandler, RespReminder respReminder)
        {           
            _discordClient = client;
            _commandHandler = commandHandler;
            _respReminder = respReminder;
        }

        //public async Task<string> Start()
        //{
        //}

        public async Task<string> Stop()
        {
            await _discordClient.StopAsync();
            
            return "Service was successfully stoped";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            if (_discordClient.ConnectionState == Discord.ConnectionState.Connected || _discordClient.ConnectionState == Discord.ConnectionState.Connecting)
            {
               
            }

            await _commandHandler.InstallCommandsAsync();
            await _discordClient.LoginAsync(Discord.TokenType.Bot, "NjEzMDYzNTk3MzEyMTgwMjM2.XVre8Q.AxGv5uirvmol1t4ZVZJVqi12K-M");
            await _discordClient.StartAsync();

            _respReminder.Start();           
        }
    }
}
