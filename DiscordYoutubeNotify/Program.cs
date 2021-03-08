using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using DiscordYoutubeNotify.Services;

namespace DiscordYoutubeNotify
{
    class Program
    {
        private readonly string _token = Environment.GetEnvironmentVariable("discord_token");
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, _token);
                await client.StartAsync();

                var commandHandling = services.GetRequiredService<CommandHandlingService>();
                commandHandling.Log += LogAsync;
                await commandHandling.InitializeAsync();

                var youtubeMonitor = services.GetRequiredService<YoutubeMonitorService>();
                youtubeMonitor.Log += LogAsync;
                await youtubeMonitor.InitializeAsync(10);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<YoutubeMonitorService>()
                .BuildServiceProvider();
        }

    }
}
