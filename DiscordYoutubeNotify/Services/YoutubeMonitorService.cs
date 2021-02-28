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

        private const string outputFile = "channels.json";
        List<YoutubeChannel> channels = new List<YoutubeChannel>();

        public HttpClient httpClient { get; set; }
        public CommandHandlingService commandHandlingService { get; set; }

        private Regex videoRegex = new Regex("{\"url\":\"/watch\\?v=([a-zA-Z0-9]*)\"");

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
                CheckYoutube(pollRateInMins);
            });
        }

        private void CheckYoutube(int pollRateInMins) {
            while (true)
            {
                for (int i = 0; i < channels.Count; i++)
                {
                    if (CheckForNewVideo(channels[i], out string newVideoUrl))
                        commandHandlingService.SendMessageAsync(channels[i].DiscordChannelId, newVideoUrl).Wait();
                }

                Task.Delay(TimeSpan.FromMinutes(pollRateInMins)).Wait();
            }
        }

        private bool CheckForNewVideo(YoutubeChannel youtubeChannel, out string newVideoUrl)
        {
            HttpResponseMessage response;
            try
            {
                response = httpClient.GetAsync(youtubeChannel.VideoUrl).Result;
            }
            catch (Exception)
            {
                Console.WriteLine($"Something went wrong when checking channel {youtubeChannel.Name}");
                newVideoUrl = null;
                return false;
            }

            string body = response.Content.ReadAsStringAsync().Result;

            var match = videoRegex.Match(body);

            if (youtubeChannel.LastVideoId != match.Groups[1].Value)
            {
                newVideoUrl = $"https://www.youtube.com/watch?v={match.Groups[1].Value}";
                youtubeChannel.LastVideoId = match.Groups[1].Value;
                SaveToDisk();
                return true;
            }
            else {
                newVideoUrl = null;
                return false;
            }
        }

        internal async Task AddNewChannel(string channelName, ulong channelId) {
            if (channels.Where(x => x.Name == channelName && channelId == x.DiscordChannelId).Count() > 0) {
                await commandHandlingService.SendMessageAsync(channelId, $"Youtube channel {channelName} has already been added.");
                return;
            }

            string successUrl = null;

            string userUrl = $"https://www.youtube.com/user/{channelName}/";
            string channelUrl = $"https://www.youtube.com/c/{channelName}/";

            string videoUrl = null;
            
            HttpResponseMessage userResponse = await httpClient.GetAsync(userUrl);

            if (userResponse.IsSuccessStatusCode)
            {
                videoUrl = $"https://www.youtube.com/user/{channelName}/videos";
                successUrl = userUrl;
            }
            else
            {
                HttpResponseMessage channelResponse = await httpClient.GetAsync(channelUrl);

                if (channelResponse.IsSuccessStatusCode)
                {
                    videoUrl = $"https://www.youtube.com/c/{channelName}/videos";
                    successUrl = channelUrl;
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

        private void SaveToDisk() {
            string output = JsonConvert.SerializeObject(channels);

            using (StreamWriter writer = File.CreateText(outputFile)){
                writer.Write(output);
            }
        }

        private void LoadFromDisk() {
            string input = File.ReadAllText(outputFile);

            channels = JsonConvert.DeserializeObject<List<YoutubeChannel>>(input);
        }
    }
}
