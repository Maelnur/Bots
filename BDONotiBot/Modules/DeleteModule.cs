using BDONotiBot.Code;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Modules
{
    [Group("del")]
    public class DeleteModule : ModuleBase<SocketCommandContext>
    {
        private readonly AppDbContext _appDbContext;

        public DeleteModule(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [Command("boss")]
        public async Task DeleteBoss(string name)
        {
            var boss = await _appDbContext.Bosses.FirstOrDefaultAsync(x => x.Name == name);
            if(boss != null)
            {
                _appDbContext.Bosses.Remove(boss);
                await _appDbContext.SaveChangesAsync();
                await Context.Channel.SendMessageAsync("Успешно: бос удален");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Ошибка: босс не найден");
            }
        }
    }
}
