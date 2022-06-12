using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordYoutubeNotify.EfCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordYoutubeNotify
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Move the database to the DataFolder if it doesn't already exist
            //This is a temporary....

            //Final DB File Location
            string finalDbLocation = Path.GetFullPath("./Data/YoutubeNotify.db");

            if (!Directory.Exists(Path.GetDirectoryName(finalDbLocation)))
                Directory.CreateDirectory(Path.GetDirectoryName(finalDbLocation));

            if (!File.Exists(finalDbLocation))
                File.Copy("./YoutubeNotify.db", finalDbLocation);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<YoutubeNotifyDbContext>();
                    services.AddScoped<EfCoreSubscriptionRepository>();
                    services.AddScoped<EfCoreChannelRepository>();

                    services.AddSingleton(new DiscordSocketConfig()
                    {
                        GatewayIntents = GatewayIntents.AllUnprivileged,
                        AlwaysDownloadUsers = true,
                        UseInteractionSnowflakeDate = false //Fix issue with time sync with Interactions
                    });
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddSingleton<InteractionsModule>();
                    services.AddSingleton<InteractionService>();
                    services.AddSingleton<MessagesService>();

                    services.AddSingleton<UploadCheckerService>();
                    services.AddSingleton<SubscriptionManagmentService>();
                    services.AddSingleton<YoutubeAPIService>();
                    services.AddSingleton<UploadCheckerService>();

                    services.AddScoped<HttpClient>();

                    services.AddLogging();

                    services.AddHostedService<YoutubeNotifyHostedService>();
                }).ConfigureLogging((_, logging) =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.ClearProviders();
                    logging.AddSimpleConsole(options => options.IncludeScopes = true);
                });
        }
    }
}