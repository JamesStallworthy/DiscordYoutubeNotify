using DiscordYoutubeNotify.EfCore;
using DiscordYoutubeNotify.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify
{
    public class SubscriptionManagmentService
    {
        private readonly EfCoreChannelRepository _channelRepository;
        private readonly EfCoreSubscriptionRepository _subscriptionRepository;
        private readonly YoutubeAPIService _youtubeAPIService;
        private readonly ILogger<SubscriptionManagmentService> _logger;

        public enum AddChannelStatus { 
            Added,
            AlreadyAdded,
            Failed
        }

        public SubscriptionManagmentService(EfCoreChannelRepository channelRepository, EfCoreSubscriptionRepository subscriptionRepository, YoutubeAPIService youtubeAPIService, ILogger<SubscriptionManagmentService> logger)
        {
            _channelRepository = channelRepository;
            _subscriptionRepository = subscriptionRepository;
            _youtubeAPIService = youtubeAPIService;
            _logger = logger;
        }

        public async Task<AddChannelStatus> AddChannel(string channelUrl, string discordChannelId) {
            try
            {
                string channelId = await _youtubeAPIService.GetChannelID(channelUrl);

                Channel channel = await _channelRepository.GetChannelWithChannelId(channelId);

                if (channel == null)
                {
                    channel = await _channelRepository.Add(new Channel()
                    {
                        ChannelId = channelId,
                        ChannelName = "TODO Need to Add",
                        LatestVideoId = null,
                        Subscriptions = new List<Subscription>() {
                        new Subscription(){
                            DiscordChannelId = discordChannelId
                        }
                    }
                    });

                    return AddChannelStatus.Added;
                }

                if (channel.Subscriptions.Where(x => x.DiscordChannelId == discordChannelId).Count() == 0)
                {
                    await _subscriptionRepository.Add(new Subscription()
                    {
                        DiscordChannelId = discordChannelId,
                        ChannelId = channel.Id
                    });


                    return AddChannelStatus.Added;
                }

                return AddChannelStatus.AlreadyAdded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to add new subscription for {channelUrl} on Discord Channel {discordChannelId}");
                return AddChannelStatus.Failed;
            }

        }
    }
}
