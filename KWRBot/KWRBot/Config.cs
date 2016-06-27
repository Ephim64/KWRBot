using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWRBot
{
    public class Config
    {
        [JsonProperty("owner_id")]
        public string OwnerID { get; internal set; }

        [JsonProperty("bot_email")]
        public string BotEmail { get; internal set; }

        [JsonProperty("bot_pass")]
        public string BotPass { get; internal set; }

        [JsonProperty("bot_token")]
        public string BotToken { get; internal set; }

        [JsonProperty("command_prefix")]
        public char CommandPrefix { get; internal set; } = '!';

        [JsonProperty("modules")]
        public Dictionary<string, bool> ModulesDictionary { get; internal set; }
    }
}
