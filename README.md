# DiscordYoutubeNotify
Discord bot that posts into a channel when a new video has been uploaded

### Bot Commands
* "!AddChannel \<channel name\>" : Add a new youtube channel for the bot to monitor and post when new videos are posted. Link to video will be posted in discord channel the command was ran in.
* "!DeleteChannel \<channel name\>" : Delete a youtube channel from the list of youtube channels the bot was monitoring. Only removes it for the current discord channel.
* "!ListChannels" : List all the youtube channels that the bot is currently monitoring for the current discord channel.

### Build steps
1. Rename the ```env-vars.env.example``` to ```env-vars.env```
2. Open ```env-vars.env``` and replace ```<INSERT_TOKEN>``` with your discord api token (https://discord.com/developers/docs/topics/oauth2)
3. Building project
    1. Use Visual Studio 2019 to build the solution in Release mode
    2. Or use dotnet command line from the root of project: ```dotnet publish -c Release```
4. If you are planning not to run via Docker then you will need to manually add the following environment variable to your system: ```discord_token=<insert_token>```
5. Run either the ```DiscordYoutubeNotify.exe``` on Windows or ```dotnet DiscordYoutubeNotify.dll``` on Linux

### Run via Docker-Compose Setup
1. From the Publish or Release folder run ```docker-compose up -d```
