using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiscordYoutubeNotify.SubscriptionManagmentService;

namespace DiscordYoutubeNotify
{
    public class InteractionsModule:InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService _commands { get; set; }
        private readonly SubscriptionManagmentService _subscriptionManagmentService;

        public InteractionsModule(InteractionService commands, SubscriptionManagmentService subscriptionManagmentService)
        {
            _commands = commands;
            _subscriptionManagmentService = subscriptionManagmentService;
        }

        [SlashCommand("subscribetochannel", "Repeat the input")]
        public async Task SubscribeToChannel(string channelUrl)
        {
            AddChannelStatus status = await _subscriptionManagmentService.AddChannel(channelUrl, Context.Channel.Id.ToString());

            if (status == AddChannelStatus.Added)
                await RespondAsync($"{channelUrl} has been subscribed to.");
            else if (status == AddChannelStatus.AlreadyAdded)
                await RespondAsync($"{channelUrl} has already been subscribed to.");
            else
                await RespondAsync("Something went wrong");
        }
    }
}
