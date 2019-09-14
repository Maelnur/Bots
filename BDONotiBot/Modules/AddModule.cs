using BDONotiBot.Code;
using BDONotiBot.Models;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Modules
{
    [Group("add")]
    public class AddModule : ModuleBase<SocketCommandContext>
    {
        private readonly AppDbContext _appDbContext;

        public AddModule(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [Command("boss")]
        public async Task AddBoss(string name)
        {            
            var result = await _appDbContext.Bosses.FirstOrDefaultAsync(x => x.Name == name);
            if(result == null)
            {
                var boss = new Boss { Name = name };
                //boss.Resps = new List<Resp>();
                await _appDbContext.AddAsync(boss);
                await _appDbContext.SaveChangesAsync();
                await Context.Channel.SendMessageAsync("Успешно: босс " + name + " добавлен");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Ошибка: босс уже существует");
            }                       
        }

        [Command("resp")]
        public async Task AddRespTime(string name, int dw, int resp)
        {
            var result = await _appDbContext.Bosses.FirstOrDefaultAsync(x => x.Name == name);
            if(result != null)
            {
                var respawnDw = result.Resps.FirstOrDefault(x => x.DayOfTheWeek == dw);
                if(respawnDw == null)
                {                                                        
                    result.Resps.Add(new Resp { BossId = result.Id, DayOfTheWeek = dw });
                    _appDbContext.Bosses.Update(result);
                    await _appDbContext.SaveChangesAsync();

                    var respawn = await _appDbContext.Resps.FirstOrDefaultAsync(x => x.DayOfTheWeek == dw && x.BossId == result.Id);
                    if(respawn != null)
                    {
                        respawn.RespTime.Add(new RespTime { RespId = respawn.Id, Resp = new DateTime(1, 1, 1, resp, 0, 0) });
                        _appDbContext.Resps.Update(respawn);
                        await _appDbContext.SaveChangesAsync();
                        await Context.Channel.SendMessageAsync("Успешно: респ добавлен");
                    }
                    
                }
                else
                {
                    respawnDw.RespTime.Add(new RespTime { RespId = respawnDw.Id, Resp = new DateTime(1, 1, 1, resp, 0, 0) });
                    _appDbContext.Resps.Update(respawnDw);
                    await _appDbContext.SaveChangesAsync();
                    await Context.Channel.SendMessageAsync("Успешно: респ добавлен");
                }
                
            }
            else
            {
                await Context.Channel.SendMessageAsync("Ошибка: босс не найден");
            }
            
        }

        [Command("noti")]
        public async Task AddNotiTime(string time)
        {
            await Context.Channel.SendMessageAsync("");
        }
    }
}
