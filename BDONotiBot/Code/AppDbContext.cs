using BDONotiBot.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BDONotiBot.Code
{
    public class AppDbContext : DbContext
    {
        public DbSet<Boss> Bosses { get; set; }
        public DbSet<Resp> Resps { get; set; }
        public DbSet<Noti> Notis { get; set; }
        public DbSet<RespTime> RespTimes { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Boss>().HasMany(x => x.Resps).WithOne().HasForeignKey(x => x.BossId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Resp>().HasMany(x => x.RespTime).WithOne().HasForeignKey(x => x.RespId).OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "DboNotiBotApp.db");
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = dbPath };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlite(connection);
        }
    }
}
