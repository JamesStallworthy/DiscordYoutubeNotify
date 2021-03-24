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
        public CommandHandlingService commandHandlingService { get; set; }

        [Command("NewChannel")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public async Task AddChannelCommand([Remainder] string channelName)
        {
            await youtubeMonitorService.AddNewChannel(channelName, Context.Channel.Id);
        }

        [Command("DeleteChannel")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public async Task DeleteChannel([Remainder] string channelName) 
        {
            await youtubeMonitorService.DeleteChannel(channelName, Context.Channel.Id);
        }

        [Command("ListChannels")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        public async Task ListChannels()
        {
            await youtubeMonitorService.ListChannels(Context.Channel.Id);
        }

        const string HelpMessage = "The following commands are available when ran from within a server channel: \r\n" +
            "!NewChannel <Youtube Channel Name/ID> :- Add a new youtube channel to follow. New videos will be posted to the channel the command is ran in.\r\n" +
            "!DeleteChannel <Youtube Channel Name/ID> :- Delete a youtube channel from the follow list. New videos will no longer be posted to the current discord channel\r\n" +
            "!ListChannels :- List the youtube channels that the current discord channel is following";

        [Command("Help")]
        [RequireContext(ContextType.DM, ErrorMessage = "Sorry, this command must be ran from a DM!")]
        public async Task Help()
        {
            await commandHandlingService.SendMessageAsync(Context.Channel.Id, HelpMessage);
        }
    }
}
