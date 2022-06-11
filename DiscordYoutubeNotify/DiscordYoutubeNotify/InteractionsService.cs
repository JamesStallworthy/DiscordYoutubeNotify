using Discord;
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

        [SlashCommand("subscribe", "Subscribe to a youtube channel")]
        public async Task SubscribeToChannel([Summary(description: "Channel Home Page URL")]string channelUrl)
        {
            AddChannelStatus status = await _subscriptionManagmentService.AddChannel(channelUrl, Context.Channel.Id.ToString());

            if (status == AddChannelStatus.Added)
                await RespondAsync($"{channelUrl} has been subscribed to.");
            else if (status == AddChannelStatus.AlreadyAdded)
                await RespondAsync($"{channelUrl} has already been subscribed to.");
            else
                await RespondAsync("Something went wrong");
        }

        [SlashCommand("showall", "Show all subscribed to channels")]
        public async Task ShowAll()
        {
            var channels = await _subscriptionManagmentService.GetAllSubscribedToChannels(Context.Channel.Id.ToString());

            if (channels.Count == 0)
                await RespondAsync("The channel has no subscriptions");

            await RespondAsync(string.Join("\r\n", channels));
        }

        [SlashCommand("unsubscribe", "Select a channel to unsubscribe from")]
        public async Task UnsubsribeSelect()
        {
            var channels = await _subscriptionManagmentService.GetAllSubscribedToChannels(Context.Channel.Id.ToString());

            if (channels.Count == 0)
                await RespondAsync("The channel has no subscriptions");

            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select a channel to unsubscribe from")
                .WithCustomId("unsub-select")
                .WithMinValues(1)
                .WithMaxValues(1);

            foreach (var channel in channels)
                menuBuilder.AddOption(channel.ChannelName, channel.ChannelId);

            var builder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);

            await RespondAsync("What channel to unsubscribe from", components: builder.Build());
        }

        //Maybe allow unsubsribe from message context menu
        //[MessageCommand("unsubscribe")]
        //public async Task Unsubscribe(IMessage msg)
        //{

        //}
    }
}
