
using DotnetGeminiSDK;
using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Config;
using DotnetGeminiSDK.Model.Response;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using StreamerBotSuzume;
using System;
using System.Net.NetworkInformation;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Net.WebRequestMethods;


class Program
{

    static void Main(string[] args)
    {
        try
        {

            #region Set up configuration and logging
            // Process the response as needed

            SystemConfig systemValues;
            MessageLogic messageLogic;
            TelegramValues telegramValues;
            GeminiValues geminiValues;

            try
            {
                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json", optional: false);

                IConfiguration config = builder.Build();


                systemValues = config.GetSection("systemConfig").Get<SystemConfig>();

                if(systemValues!=null && systemValues?.enableLogging != null)
                {
                    Logger.enableLogging = (bool)systemValues?.enableLogging!;
                }

                Logger.LogMessage($"---------New Instance---------");
                messageLogic = config.GetSection("messageLogic").Get<MessageLogic>();
                telegramValues = config.GetSection("telegramValues").Get<TelegramValues>();
                geminiValues = config.GetSection("geminiValues").Get<GeminiValues>();
            }
            catch (Exception ex)
            {
                Logger.LogError("| Config Error - " + ex.Message);
                return;
            }



            Logger.LogMessage($"twitchUrl {messageLogic?.twitchUrl}");
            Logger.LogMessage($"messageBase {messageLogic?.messageBase}");
            Logger.LogMessage($"messageWithAi {messageLogic?.messageWithAi}");
            Logger.LogMessage($"useAi {messageLogic?.useAi}");

            Logger.LogMessage($"chatId {telegramValues?.chatId}");
            Logger.LogMessage($"token {telegramValues?.token}");
            //Logger.LogMessage($"adminChatId {telegramValues?.adminChatId}");

            Logger.LogMessage($"geminiApiKey {geminiValues?.geminiApiKey}");
            Logger.LogMessage($"textBaseUrl {geminiValues?.textBaseUrl}");
            Logger.LogMessage($"prompt {geminiValues?.prompt}");


            //Logger.LogMessage($"logPath {systemValues?.logPath}");
            Logger.LogMessage($"enableLogging {systemValues?.enableLogging}");

            ////////////////////////////////////////////////////////////////////////////
            if (string.IsNullOrEmpty(messageLogic?.twitchUrl))
            {
                Logger.LogMessage("twitchUrl is null");
            }
            if (string.IsNullOrEmpty(messageLogic?.messageBase))
            {
                Logger.LogError("messageBase is null - Please, add messageBase to config.json");
                return;
            }
            if (messageLogic?.useAi == true && string.IsNullOrEmpty(messageLogic?.messageWithAi))
            {
                Logger.LogError("messageWithAi is null but you tried to use Ai. Please, fill messageWithAi or switch off useAi = false");
                return;
            }
            if (string.IsNullOrEmpty(messageLogic?.messageBaseWithArgs))
            {
                Logger.LogMessage("messageBaseWithArgs is null");
            }
            ////////////////////////////////////////////////////////////////////////////
            if (string.IsNullOrEmpty(telegramValues?.chatId))
            {
                Logger.LogMessage("chatId is null, Telegram will not work");
            }
            if (string.IsNullOrEmpty(telegramValues?.token))
            {
                Logger.LogMessage("token is null, Telegram will not work");
            }
            ////////////////////////////////////////////////////////////////////////////
            if (string.IsNullOrEmpty(geminiValues?.geminiApiKey))
            {
                Logger.LogMessage("geminiApiKey is null");
            }
            if (string.IsNullOrEmpty(geminiValues?.textBaseUrl))
            {
                Logger.LogMessage("textBaseUrl is null");
            }
            if (string.IsNullOrEmpty(geminiValues?.prompt) && messageLogic?.useAi == true)
            {
                Logger.LogMessage("prompt is null");
            }
            ////////////////////////////////////////////////////////////////////////////  
            #endregion


            Dictionary<string, string> dict = new Dictionary<string, string>();
            int countArgs = 0;
            foreach (var item in args)
            {
                Logger.LogMessage(item);
                dict.Add(countArgs.ToString(), item);
                countArgs++;
            }

            #region set up variables for replacement
            foreach (var item in dict)
            {
                if (messageLogic?.useAi == true && !string.IsNullOrEmpty(messageLogic?.messageWithAi))
                {
                    messageLogic.messageWithAi = replaceString(messageLogic.messageWithAi, item.Key, item.Value);
                    if (!string.IsNullOrEmpty(geminiValues?.prompt))
                    {
                        geminiValues.prompt = replaceString(geminiValues.prompt, item.Key, item.Value);
                    }
                }

                if (!string.IsNullOrEmpty(messageLogic?.messageBaseWithArgs))
                {
                    messageLogic.messageBaseWithArgs = replaceString(messageLogic.messageBaseWithArgs, item.Key, item.Value);
                }

                if (!string.IsNullOrEmpty(messageLogic?.messageBase))
                {
                    messageLogic.messageBase = replaceString(messageLogic.messageBase, item.Key, item.Value);
                }
            }

            if (!string.IsNullOrEmpty(messageLogic?.twitchUrl))
            {
                messageLogic.messageBase = replaceString(messageLogic?.messageBase!, "twitchUrl", messageLogic?.twitchUrl!);

                if (!string.IsNullOrEmpty(messageLogic?.messageBaseWithArgs))
                {
                    messageLogic.messageBaseWithArgs = replaceString(messageLogic.messageBaseWithArgs, "twitchUrl", messageLogic.twitchUrl);
                }


                if (messageLogic?.useAi == true && !string.IsNullOrEmpty(messageLogic?.messageWithAi))
                {
                    messageLogic.messageWithAi = replaceString(messageLogic.messageWithAi, "twitchUrl", messageLogic.twitchUrl);
                }
            }
            #endregion

            string aiMessage = null!;

            if (messageLogic?.useAi == true && !string.IsNullOrEmpty(geminiValues?.prompt) && !string.IsNullOrEmpty(geminiValues?.geminiApiKey) && !string.IsNullOrEmpty(geminiValues?.textBaseUrl))
            {
                try
                {
                    var Gemini = new Gemini(geminiValues?.geminiApiKey!, geminiValues?.textBaseUrl!);
                    var geminiEx = Gemini.GetResponse(geminiValues?.prompt!);

                    var result = geminiEx.Result;

                    aiMessage = result.Candidates.First()?.Content?.Parts?.FirstOrDefault()?.Text!;

                    var resultMessage = aiMessage;
                    if (!string.IsNullOrEmpty(aiMessage) && !string.IsNullOrEmpty(messageLogic?.messageWithAi))
                    {
                        messageLogic.messageWithAi = replaceString(messageLogic.messageWithAi, "aiMessage", aiMessage);
                        resultMessage = messageLogic.messageWithAi;

                        TelegramCore.SendMessageToTelegram(aiMessage, telegramValues);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("| Gemini Error - " + ex.Message);
                }
            }
            else
            {
                if (args.Length > 0 && !string.IsNullOrEmpty(messageLogic?.messageBaseWithArgs))
                {
                    TelegramCore.SendMessageToTelegram(messageLogic?.messageBaseWithArgs!, telegramValues!);
                }
                else
                {
                    TelegramCore.SendMessageToTelegram(messageLogic?.messageBase!, telegramValues!);
                }
            }

            Logger.LogMessage($"-----------End-----------\n");
        }
        catch (Exception ex)
        {
            Logger.LogError("| Main Error - " + ex.Message);
        }

    }

    public static string replaceString(string stringToEdit, string whatReplace, string replacedValue)
    {
        string finalString = stringToEdit.Replace("{" + whatReplace + "}", replacedValue);
        return finalString;
    }
}