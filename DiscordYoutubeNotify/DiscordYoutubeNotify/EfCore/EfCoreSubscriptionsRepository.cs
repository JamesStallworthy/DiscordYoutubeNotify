using DiscordYoutubeNotify.EfCore.Base;
using DiscordYoutubeNotify.Models;
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
    }
}
