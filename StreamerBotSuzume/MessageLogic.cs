using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamerBotSuzume
{
    public class MessageLogic
    {
        public bool? useAi { get; set; } = false;
        public string? messageWithAi { get; set; } = null!;
        public string? messageBaseWithArgs { get; set; } = null!;
        public string? messageBase { get; set; } = null!;
        public string? twitchUrl { get; set; } = null!;
    }
}
