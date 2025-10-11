using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Config;
using DotnetGeminiSDK.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamerBotSuzume
{
    public class Gemini
    {
        private readonly GeminiClient _geminiClient;

        public Gemini(string apikey, string textBaseUrl)
        {
            _geminiClient = new GeminiClient(new GoogleGeminiConfig()
            {
                ApiKey = apikey,
                TextBaseUrl = textBaseUrl
            });
        }


        public async Task<GeminiMessageResponse> GetResponse(string text)
        {
            var response = await _geminiClient.TextPrompt(text);
            return response;
            // Process the response as needed
        }
    }
}
