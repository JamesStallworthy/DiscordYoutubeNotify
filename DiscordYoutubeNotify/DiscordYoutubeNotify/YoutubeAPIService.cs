using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify
{
    public class YoutubeAPIService
    {
        public string APIKey;

        private string GetLatestVideo = "https://youtube.googleapis.com/youtube/v3/playlistItems?part=contentDetails&maxResults=1&playlistId={0}&key={1}]";

        private readonly HttpClient _httpClient;

        public YoutubeAPIService(HttpClient httpClient)
        {
            APIKey = Environment.GetEnvironmentVariable("youtube_token");
            _httpClient = httpClient;
        }

        private readonly Regex regex = new Regex("<link rel=\"canonical\" href=\"https:\\/\\/www.youtube.com\\/channel\\/([a-zA-Z0-9_-]+)\">");

        public async Task<string> GetChannelID(string channelUrl) {
            string host = new Uri(channelUrl).Host;

            if (host != "www.youtube.com")
                throw new Exception("Non Youtube URL has been passed");

            var response = await _httpClient.GetAsync(channelUrl);

            var body = await response.Content.ReadAsStringAsync();

            var result = regex.Match(body);

            if (result.Groups.Count == 2) //First is the whole canonical and second group is the channel id
                return result.Groups[1].Value;

            throw new Exception("Invalid Youtube URL has been passed");
        }

        public string GetUploadPlaylistId(string channelId) {
            string playlistId = "UU" + channelId.Remove(0, 2);
            return playlistId;
        }

        public async Task<string> GetLatestVideoId(string channelID) {
            string builtUrl = string.Format(GetLatestVideo, GetUploadPlaylistId(channelID), APIKey);

            var response = await _httpClient.GetAsync(builtUrl);

            var content = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<PlaylistItemsJson>(content);

            if (json != null)
                return json.items[0].contentDetails.videoId;
            else
                throw new Exception("Failed to retrieve latest video");
        }

        private class PlaylistItemsJson {
            public List<PlaylistItem> items { get; set; }
        }

        private class PlaylistItem { 
            public ContentDetails contentDetails { get; set; }
        }

        private class ContentDetails {
            public DateTime videoPublishedAt { get; set; }
            public string videoId { get; set; }
            
        }
    }
}
