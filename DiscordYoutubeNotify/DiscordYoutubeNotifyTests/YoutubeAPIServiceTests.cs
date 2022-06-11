using DiscordYoutubeNotify;
using RichardSzalay.MockHttp;

namespace DiscordYoutubeNotifyTests
{
    public class YoutubeAPIServiceTests
    {
        private YoutubeAPIService _service;

        [SetUp]
        public void Setup()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When("https://youtube.googleapis.com/youtube/v3/playlistItems*")
                    .Respond("application/json", File.ReadAllText("./LatestVideoApiResponse.txt"));

            mockHttp.When("https://youtube.googleapis.com/youtube/v3/channels*")
                .Respond("application/json", File.ReadAllText("./ChannelApiResponse.txt"));

            mockHttp.When("https://www.youtube.com/c/UKDashCameras").
                Respond("text/html", File.ReadAllText("./ChannelHomepage.txt"));

            _service = new YoutubeAPIService(new HttpClient(mockHttp));
        }

        [Test]
        public async Task GetChannelID()
        {
            var id = await _service.GetChannelID("https://www.youtube.com/c/UKDashCameras");

            Assert.That(id, Is.EqualTo("UCfZwNd8i7spJmLr4qq1_Gyw"));
        }

        [Test]
        public async Task GetChannelIDInvalidURL()
        {
            try
            {
                var id = await _service.GetChannelID("https://www.google.co.uk");
                Assert.Fail("Method was expected to throw an exception");
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Non Youtube URL has been passed"));
            }
        }

        [Test]
        public async Task GetChannelIDInvalidYoutubeURL()
        {
            try
            {
                var id = await _service.GetChannelID("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
                Assert.Fail("Method was expected to throw an exception");
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Invalid Youtube URL has been passed"));
            }
        }

        //TODO Add Invalid response
        [Test]
        public async Task GetLatestVideo()
        {
            var id = await _service.GetLatestVideoId("UCfZwNd8i7spJmLr4qq1_Gyw");
            Assert.That(id, Is.EqualTo("eAIulQOTDHI"));
        }

        [Test]
        public async Task GetChannelName()
        {
            var id = await _service.GetChannelName("UCfZwNd8i7spJmLr4qq1_Gyw");
            Assert.That(id, Is.EqualTo("Google Developers"));
        }


        [Test]
        public void GetPlaylistId()
        {
            var id = _service.GetUploadPlaylistId("UCfZwNd8i7spJmLr4qq1_Gyw");
            Assert.That(id, Is.EqualTo("UUfZwNd8i7spJmLr4qq1_Gyw"));
        }
    }
}