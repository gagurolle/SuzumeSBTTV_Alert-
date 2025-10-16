# SuzumeSBTTV_Alert
suzumes streamer bot telegram twitch alert

Send notifications to Telegram when you start your Streaming! Just connect it with streamer.bot 

<h1>Installation</h1>
<li>Download dotnet 8 if you dont have it <a href="https://dotnet.microsoft.com/en-us/download/dotnet/8.0">.NET 8</href></li>
<li>Download latest release or compile it yoursefl</li>
<li>Unzip to any folder</li>
<li>Connect with streamer.bot in subactions - Run a programm: <img width="1083" height="943" alt="image" src="https://github.com/user-attachments/assets/f73e46da-e5df-4984-a40b-027848e68ee8" />
</li>
<li>Set Target and Working Directory fields. Add any arguments like "%game%", "%user%" and etc... <img width="594" height="570" alt="image" src="https://github.com/user-attachments/assets/465389f2-73d5-4b5e-80e6-fd6f4144209f" /></li>
<li>Edit config file </li>
<li>Setup your streamer bot trigger with Type - Stream Online (Or create your own logic)</li>

That's All!

<h2>Usage</h2>
<h1>Gemini</h1>
if you would like to use AI to create a message - set 
"messageLogic": {
    "useAi": true },
    
otherwise set "useAi": false;

For proper usage fill this fields in "geminiValues" 
```
"geminiValues": {
    "geminiApiKey": "apiKey",
    "textBaseUrl": "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash",
    "prompt": "Create a message for my tg channel"
  }
```

1. Get apiKey its free in base plan: <a href="https://aistudio.google.com/api-keys">Gemini Studio</href></li>
2. Choose your model and place it in textBaseUrl, i use 2.0, you can use the latest. ex: https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash
3. Create a prompt - the answer form Gemini will be retranslated to you the variable {aiMessage}

<h1>Telegram</h1>

1. Create a bot via <a href="https://telegram.me/BotFather">BotFather</a>
2. Set bot token in "token": "here"
3. Set "chatId": "@your_chat_name". Just place "@" before your name of the chat, which you can usually see in link: t.me/myChannelName
4. Add your bot to your channel\chat and give him admin rights
5. That's all, everything should work fine. Please, make your first runs on test channels.

<h1>Arguments</h1>

In fields such as prompt, messageWithAi, messageBaseWithArgs and messageBase you can use arguments. 
While running application the values will fill the 
You should use brackets "{0}". 

Technical variables: 
    {aiMessage} - the message created from Gemini
    {twitchUrl} - your twitch url
    {0}, {1}, {2}, {3} ... etc - input arguments, that you defined in console or your streamerbot ("%game%", "%user%", "%date%", "placeany" ... in your streamerbot that will go directly to this app). You can define any number of variables.
    So, you can create a prompt to ai - "Create a message to invite my followers to watch my game - {0}", send "%game%" from streamerBot and it will be "Create a message to invite my followers to watch my game - Hollow Knight: Silksong".
    Also it will work in other fields simultaniosly.

<h1>Meaning of the fields</h1>

If you use Ai - the "messageWithAi" field will be translated to Telegram. If not - the "messageBaseWithArgs". If you don't have input args ({0}, {1} etc...) so the app will use "messageBase". 
In case of any errors, messages will be sent in descending order: First, messageWithAi; if there is an API access error, then messageBaseWithArgs will be sent; if there are no arguments, then messageBase will be sent.

<h1>Log</h1>
The log file will contain requests and errors. Log filw will be placed in the app directory. If you would like to disable logging:

  "enableLogging": false



    
