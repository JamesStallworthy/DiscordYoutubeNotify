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
    public class EfCoreSubscriptionRepository: EfCoreRepository<Subscription, YoutubeNotifyDbContext>
    {
        public EfCoreSubscriptionRepository(YoutubeNotifyDbContext context) : base(context)
        {

        }

        public virtual async Task<Subscription> Get(string channelId, string discordChannelId)
        {
            return await context.Set<Subscription>().Include(x => x.Channel).Where(x => x.Channel.ChannelId == channelId && x.DiscordChannelId == discordChannelId).FirstOrDefaultAsync();
        }
    }
}
