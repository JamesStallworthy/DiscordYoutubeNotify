using Discord;
using Discord.Commands;
using DiscordYoutubeNotify.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        public YoutubeMonitorService youtubeMonitorService { get; set; }

        [Command("NewChannel")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public async Task AddChannelCommand([Remainder] string channelName)
        {
            await youtubeMonitorService.AddNewChannel(channelName, Context.Channel.Id);
        }
    }
}
