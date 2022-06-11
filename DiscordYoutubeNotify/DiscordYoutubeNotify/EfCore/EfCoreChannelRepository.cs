using DiscordYoutubeNotify.EfCore.Base;
using DiscordYoutubeNotify.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify.EfCore
{
    public class EfCoreChannelRepository: EfCoreRepository<Channel, YoutubeNotifyDbContext>
    {
        public EfCoreChannelRepository(YoutubeNotifyDbContext context) : base(context)
        {

        }

        public async Task<Channel> GetChannelWithChannelId(string channelId) {
            return await context.Set<Channel>()
                .Where(x => x.ChannelId == channelId)
                .Include(x => x.Subscriptions)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Channel>> GetChannelForDiscordChannelId(string discrodChannelId)
        {
            return await context.Set<Channel>()
                .Include(x => x.Subscriptions)
                .Where(x => x.Subscriptions.Where(x => x.DiscordChannelId == discrodChannelId).Count() > 0)
                .ToListAsync();
        }

        public override async Task<List<Channel>> GetAll()
        {
            return await context.Set<Channel>().Include(x => x.Subscriptions).ToListAsync();
        }
    }
}
