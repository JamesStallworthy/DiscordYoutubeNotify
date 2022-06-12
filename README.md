# DiscordYoutubeNotify
Discord bot that posts into a channel when a new video has been uploaded

### Bot Commands
* "/subscribe channel-url:\<url to channel\>" : Subscribe to a youtube channel. The url must be to the channels home page
* "/unsubscribe" : Select a channel to unsubscribe from. This will display a list of channels that are currently subscribed to. Selecting one will unsubscribe the current discord channel.
* "/showall" : List all the youtube channels that the bot is currently monitoring for the current discord channel.

### How to run the bot
The best way to run the bot is via docker/docker-compose. 
Here is an example docker-compose file.

```yml
version: '3'

services:
  db:
    container_name: discordbotV2
    image: ghcr.io/jamesstallworthy/discordyoutubenotify:latest
    restart: always
    environment:
      - discord_token=<Discord API Token> #(https://discord.com/developers/docs/topics/oauth2)
      - youtube_pollrate=5 #How often the bot polls the youtube api in minutes
      - youtube_token=<Youtube API Key> #(https://developers.google.com/youtube/v3/getting-started)
    volumes:
      - ./data:/app/Data
```

### Discord OAuth permissions required
![image](https://user-images.githubusercontent.com/10096828/173236262-b23f542e-92f0-418b-94be-ca5890461d1b.png)