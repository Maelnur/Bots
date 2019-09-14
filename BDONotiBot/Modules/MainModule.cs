using BDONotiBot.Code;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Modules
{   
    //[Group("main")]
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        //private IAudioClient _audioClient;
        private RespReminder _respReminder;

        public MainModule()
        {
            
        }

        [Command("hello")]
        public async Task Hello()
        {
            await ReplyAsync("Привет! Я BDONotiBot");
            
        }

        [Command("инфо")]
        [Alias("info")]
        public async Task GetInfo()
        {                                 
            await Context.Channel.SendMessageAsync("");                        
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("Ошибка: пользователь должен быть в голосовом канале");
                return;
            }

            try
            {
                RespReminder.AudioClients.Add(await channel.ConnectAsync());
            }
            catch(Exception exc)
            {

            }
        }

        private Process GetProcess(string path)
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

        private async Task SendAsync(string[] path)
        {
            using (var discord = RespReminder.AudioClients[0].CreatePCMStream(AudioApplication.Mixed))
            {
                foreach (var item in path)
                {
                    using (var ffmpeg = GetProcess(item))
                    using (var output = ffmpeg.StandardOutput.BaseStream)


                        try
                        {
                            await output.CopyToAsync(discord);
                        }
                        catch (Exception exc)
                        {
                            ffmpeg.Close();
                            output.Close();
                            discord.Flush();

                        }
                        finally
                        {
                            await discord.FlushAsync();
                        }

                }
            }
                
        }
    }
}
