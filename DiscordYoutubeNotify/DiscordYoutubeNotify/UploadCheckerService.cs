using DiscordYoutubeNotify.EfCore;
using DiscordYoutubeNotify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify
{
    public class UploadCheckerService
    {
        private const string VideoURL = "https://www.youtube.com/watch?v={0}";

        private readonly EfCoreChannelRepository _channelRepository;
        private readonly YoutubeAPIService _youtubeAPIService;
        private readonly MessagesService _messagesService;
        public UploadCheckerService(EfCoreChannelRepository channelRepository, YoutubeAPIService youtubeAPIService, MessagesService messagesService)
        {
            _channelRepository = channelRepository;
            _youtubeAPIService = youtubeAPIService;
            _messagesService = messagesService;
        }

        public async Task PollChannels() {
            List<Channel> allChannels = await _channelRepository.GetAll();

            foreach (Channel channel in allChannels) {
                string latestVideoId = await _youtubeAPIService.GetLatestVideoId(channel.ChannelId);

                if (channel.LatestVideoId != latestVideoId) { //We have a new video!
                    channel.LatestVideoId = latestVideoId;

                    await _channelRepository.Update(channel);

                    foreach (Subscription sub in channel.Subscriptions)
                    {
                        await _messagesService.SendMessageAsync(ulong.Parse(sub.DiscordChannelId), string.Format(VideoURL, channel.LatestVideoId));
                    }
                }
            }
        }
    }
}
