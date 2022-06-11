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
    }
}
