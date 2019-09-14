using BDONotiBot.Models;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BDONotiBot.Code
{
    public class RespReminder
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        private List<Boss> Bosses { get; set; }
        private List<Noti> NotiTime { get; set; }
        public static List<IAudioClient> AudioClients { get; set; }

        public RespReminder(DiscordSocketClient client, IServiceProvider services)
        {
            _serviceProvider = services;
            _discordClient = client;
            using (var scoped = _serviceProvider.CreateScope())
            {
                AudioClients = new List<IAudioClient>();
                var db = ActivatorUtilities.CreateInstance<AppDbContext>(scoped.ServiceProvider);
                Bosses = db.Bosses.ToList();
                NotiTime = db.Notis.ToList();
            }
        }

        public void Start()
        {
            _timer = new Timer(TimerCallback, null, 0, 60000);
        }

        public void Stop()
        {
            _timer.Dispose();
        }

        public async void TimerCallback(object obj)
        {
            int wdNumber = ((int)DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
            int nextWdNumber = wdNumber + 1 == 8 ? 1 : wdNumber + 1;
            var tsNow = TimeSpan.Parse(DateTime.Now.Hour + ":" + DateTime.Now.Minute);
  
            foreach(var noti in NotiTime)
            {
                var boss = Bosses.Where(x =>
                                        x.Resps.Any(y =>
                                        y.DayOfTheWeek == wdNumber && y.RespTime.Any(z =>
                                        (TimeSpan.Parse(z.Resp.Hour + ":00") - tsNow).Hours == 0 && 
                                        (TimeSpan.Parse(z.Resp.Hour + ":00") - tsNow).Minutes == noti.NotiTime))).ToList();

                var nextDayBoss = Bosses.Where(x => 
                                               x.Resps.Any(y => 
                                               y.DayOfTheWeek == nextWdNumber && y.RespTime.Any(z =>
                                               (TimeSpan.Parse(z.Resp.Hour + ":00") - tsNow).Hours == -23 &&
                                               (TimeSpan.Parse(z.Resp.Hour + ":00") - tsNow).Minutes == (60 - noti.NotiTime) * -1))).ToList();
               

                if (boss.Count > 0 || nextDayBoss.Count > 0)
                {
                    var guilds = _discordClient.Guilds;
                    foreach (var guild in guilds)
                    {
                        var ch = guild.Channels.SingleOrDefault(x => x.Name == "main") as ISocketMessageChannel;
                        if (ch != null)
                        {
                            var currentBoss = boss.Count > 0 ? boss : nextDayBoss;
                            foreach(var item in currentBoss)
                            {
                                if(noti.NotiTime != 0)
                                {
                                    await ch.SendMessageAsync("Босс " + item.Name + " пробудится через " + noti.NotiTime + " минут");
                                    if(AudioClients.Count > 0)
                                    {
                                        string[] path = { "sounds/внимание.mp3", "sounds/" + item.Name + ".mp3", "sounds/пробудится.mp3", "sounds/" + noti.NotiTime + ".mp3" };
                                        foreach(var client in AudioClients)
                                        {
                                            await SendAsync(client, path);
                                        }
                                    }
                                }
                                else
                                {
                                    await ch.SendMessageAsync("Босс " + item.Name + " пробудился");
                                }
                                
                            }
                           
                        }
                    }
                }
            }                   
        }

        private async Task SendAsync(IAudioClient client, string[] path)
        {
            using (var discord = client.CreatePCMStream(AudioApplication.Voice))
            {
                foreach(var item in path)
                {
                    using (var ffmpeg = GetFfmpeg(item))
                    using (var output = ffmpeg.StandardOutput.BaseStream)
                    {
                        try
                        {
                            await output.CopyToAsync(discord);
                        }
                        catch (Exception exc)
                        {
                            ffmpeg.Close();
                            output.Close();
                            await discord.FlushAsync();

                        }
                        finally
                        {
                            await discord.FlushAsync();
                        }
                    }
                }              
            }                           
        }

        private Process GetFfmpeg(string path)
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                });
                return process;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
    }
}
