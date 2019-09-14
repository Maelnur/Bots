using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDONotiBot.Code;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BDONotiBot
{
    public class Startup
    {
        //public IConfiguration Apponfiguration { get; set; }

        //public Startup(IHostingEnvironment env)
        //{
            
        //}

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<AppDbContext>();
            services.AddHostedService<BotApp>();
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<AppCommandHandler>();
            services.AddSingleton<RespReminder>();
        }
    
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider  provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                
            }           
        }
    }
}
