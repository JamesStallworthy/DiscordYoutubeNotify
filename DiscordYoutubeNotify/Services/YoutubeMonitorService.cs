using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify.Services
{
    public class YoutubeMonitorService
    {
        class YoutubeChannel
        {
            public string Name { get; set; }
            public string ChannelUrl { get; set; }
            public string VideoUrl { get; set; }
            public ulong DiscordChannelId { get; set; }
            public string LastVideoId { get; set; } = null;
        }

        public event Func<LogMessage, Task> Log;

        private const string configFolder = "Config";
        private const string outputFile = "Config/channels.json";
        List<YoutubeChannel> channels = new List<YoutubeChannel>();

        public HttpClient httpClient { get; set; }
        public CommandHandlingService commandHandlingService { get; set; }

        private Regex videoRegex = new Regex("{\"url\":\"/watch\\?v=([a-zA-Z0-9\\-]*)\"");

        public YoutubeMonitorService(HttpClient _httpClient, CommandHandlingService _commandHandlingService)
        {
            httpClient = _httpClient;
            commandHandlingService = _commandHandlingService;
        }

        public async Task InitializeAsync(int pollRateInMins)
        {
            LoadFromDisk();
            await Task.Run(() =>
            {
                while (true)
                {
                    CheckYoutube().Wait();

                    Task.Delay(TimeSpan.FromMinutes(pollRateInMins)).Wait();
                }
            });
        }

        private async Task CheckYoutube() 
        {
            await Log(new LogMessage(LogSeverity.Info, "CheckYoutube", $"Checking for new youtube videos"));
            for (int i = 0; i < channels.Count; i++)
            {
                await CheckForNewVideo(channels[i]);
            }
        }

        private async Task CheckForNewVideo(YoutubeChannel youtubeChannel)
        {
            HttpResponseMessage response;
            try
            {
                response = httpClient.GetAsync(youtubeChannel.VideoUrl).Result;
            }
            catch (Exception ex)
            {
                await Log(new LogMessage(LogSeverity.Error, "CheckForNewVideo", $"Something went wrong when checking channel {youtubeChannel.Name}", ex));
                return;
            }

            string body = response.Content.ReadAsStringAsync().Result;

            var match = videoRegex.Match(body);

            if (youtubeChannel.LastVideoId == null) {
                youtubeChannel.LastVideoId = match.Groups[1].Value;
                SaveToDisk();
                return;
            }
            else if (youtubeChannel.LastVideoId != match.Groups[1].Value)
            {
                string newVideoUrl = $"https://www.youtube.com/watch?v={match.Groups[1].Value}";
                await Log(new LogMessage(LogSeverity.Info, "CheckForNewVideo", $"New Youtube video for: {youtubeChannel.Name}"));
                if (await commandHandlingService.SendMessageAsync(youtubeChannel.DiscordChannelId, newVideoUrl)) {
                    youtubeChannel.LastVideoId = match.Groups[1].Value;
                    SaveToDisk();
                }
                return;
            }
        }

        internal async Task AddNewChannel(string channelName, ulong channelId) {
            if (channels.Where(x => x.Name == channelName && channelId == x.DiscordChannelId).Count() > 0) {
                await commandHandlingService.SendMessageAsync(channelId, $"Youtube channel {channelName} has already been added.");
                return;
            }

            string successUrl = null;

            string[] channelUrlFormats = new string[] {
                $"https://www.youtube.com/user/{channelName}/",
                $"https://www.youtube.com/c/{channelName}/",
                $"https://www.youtube.com/channel/{channelName}/"
            };

            string videoUrl = null;

            foreach (var url in channelUrlFormats)
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode) {
                    videoUrl = $"https://www.youtube.com/user/{channelName}/videos";
                    successUrl = url;
                }
            }

            if (string.IsNullOrEmpty(videoUrl)) {
                await commandHandlingService.SendMessageAsync(channelId, $"Invalid youtube channel name: {channelName}.");
                return;
            }

            channels.Add(new YoutubeChannel()
            {
                Name = channelName,
                ChannelUrl = successUrl,
                VideoUrl = videoUrl,
                DiscordChannelId = channelId
            });

            await commandHandlingService.SendMessageAsync(channelId, $"Added youtube channel: {successUrl}.");
            SaveToDisk();
        }

        internal async Task DeleteChannel(string channelName, ulong channelId)
        {
            if (channels.RemoveAll(x => x.Name == channelName && x.DiscordChannelId == channelId) > 0)
            {
                await commandHandlingService.SendMessageAsync(channelId, $"No longer following: {channelName}.");
            }
            else {
                await commandHandlingService.SendMessageAsync(channelId, $"Not following {channelName} so was not removed.");
            }
            SaveToDisk();
        }

        internal async Task ListChannels(ulong channelId)
        {
            StringBuilder channelsText = new StringBuilder();
            channelsText.AppendLine();

            channels.ForEach(x => channelsText.AppendLine($"{x.Name} - {x.ChannelUrl}"));

            await commandHandlingService.SendMessageAsync(channelId, $"The following channels are being followed:" + channelsText.ToString());
        }

        private void SaveToDisk() {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            string output = JsonConvert.SerializeObject(channels);

            using (StreamWriter writer = File.CreateText(outputFile)){
                writer.Write(output);
            }
        }

        private void LoadFromDisk() {
            if (File.Exists(outputFile))
            {
                string input = File.ReadAllText(outputFile);

                channels = JsonConvert.DeserializeObject<List<YoutubeChannel>>(input);
            }
        }
    }
}
