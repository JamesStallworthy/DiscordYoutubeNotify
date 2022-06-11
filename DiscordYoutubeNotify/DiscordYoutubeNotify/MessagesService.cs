using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify
{
    public class MessagesService
    {
        private readonly DiscordSocketClient _discordSocketClient;

        public MessagesService(DiscordSocketClient discordSocketClient)
        {
            _discordSocketClient = discordSocketClient;
        }

        public async Task SendMessageAsync(ulong discordChannelId, string message) {
            var channel = await _discordSocketClient.GetChannelAsync(discordChannelId) as IMessageChannel;

            await channel.SendMessageAsync(message);
        }
    }
}
