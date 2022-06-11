using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify
{
    public class YoutubeNotifyHostedService : IHostedService
    {
        private readonly ILogger<YoutubeNotifyHostedService> _logger;
        private readonly ILogger<DiscordSocketClient> _loggerDiscordSocketClient;

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _services;

        public YoutubeNotifyHostedService(ILogger<YoutubeNotifyHostedService> logger, ILogger<DiscordSocketClient> loggerDiscordSocketClient,
            DiscordSocketClient discordSocketClient, InteractionService interactionService, IServiceProvider services)
        {
            _logger = logger;
            _loggerDiscordSocketClient = loggerDiscordSocketClient;
            _client = discordSocketClient;
            _interactionService = interactionService;
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");

            _client.Log += DiscordClientLogAsync;
            _client.Ready += ReadyAsync;

            string discordToken = Environment.GetEnvironmentVariable("discord_token");

            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();

            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.InteractionCreated += HandleInteraction;

            //var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            //while (await timer.WaitForNextTickAsync())
            //{
            //    //Refresh the youtube api
            //}
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping");
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
            // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
#if DEBUG
            await _interactionService.RegisterCommandsToGuildAsync(815284605208231937, true);
#else
            await _handler.RegisterCommandsGloballyAsync(true);
#endif
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
                var context = new SocketInteractionContext(_client, interaction);

                // Execute the incoming command.
                var result = await _interactionService.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // implement
                            break;
                        default:
                            break;
                    }
            }
            catch
            {
                // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }

        //Map Discord.Net Log Messages to ILogger
        //Why they don't bind these together to begin with is odd.
        private Task DiscordClientLogAsync(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    _loggerDiscordSocketClient.LogCritical(msg.ToString());
                    break;
                case LogSeverity.Error:
                    _loggerDiscordSocketClient.LogError(msg.ToString());
                    break;
                case LogSeverity.Warning:
                    _loggerDiscordSocketClient.LogWarning(msg.ToString());
                    break;
                case LogSeverity.Info:
                    _loggerDiscordSocketClient.LogInformation(msg.ToString());
                    break;
                case LogSeverity.Verbose:
                    _loggerDiscordSocketClient.LogTrace(msg.ToString());
                    break;
                case LogSeverity.Debug:
                    _loggerDiscordSocketClient.LogDebug(msg.ToString());
                    break;
                default:
                    _loggerDiscordSocketClient.LogInformation(msg.ToString());
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
