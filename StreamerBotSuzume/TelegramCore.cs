using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace StreamerBotSuzume
{
    public class TelegramCore
    {
        private static TelegramBotClient bot = null!;

        public static string admin_chat_id = null!;
        public TelegramCore(string token = null!, string _admin_chat_id = null!)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }
            bot = new TelegramBotClient(token);
            admin_chat_id = _admin_chat_id;
        }

        public static async Task SendOneMessageToChannelAsync(string text = null!, string chat_id = null!)
        {
            if (bot != null && text != null)
                await bot.SendMessage(chat_id, text);
        }
        public static void SendMessageToTelegram(string message, TelegramValues values)
        {
            try
            {
                if (string.IsNullOrEmpty(values?.chatId) || string.IsNullOrEmpty(values?.token) || string.IsNullOrEmpty(values?.adminChatId))
                {
                    Logger.LogMessage("Telegram values are null");
                    return;
                }

                new TelegramCore(values?.token!, values?.adminChatId!);

                Logger.LogMessage(message);

                TelegramCore.SendOneMessageToChannelAsync(message, values?.chatId!).Wait();

                Task.Delay(1000).Wait();
            }
            catch (Exception ex)
            {
                Logger.LogError("| Telegram Error - " + ex.Message);
            }
        }

    }
}
