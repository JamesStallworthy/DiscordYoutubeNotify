using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordYoutubeNotify.Models;

namespace DiscordYoutubeNotify.EfCore
{
    public class YoutubeNotifyDbContext : DbContext
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public string DbPath { get; } = "./Data/YoutubeNotify.db";

        public YoutubeNotifyDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Channel>();

            modelBuilder.Entity<Subscription>();
        }
    }
}
