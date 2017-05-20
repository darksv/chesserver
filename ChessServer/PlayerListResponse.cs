using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChessServer
{
    public class PlayerListResponse
    {
        [JsonProperty(PropertyName = "response")]
        public string Response => "presence";
        
        [JsonProperty(PropertyName = "players")]
        public List<Player> Players { get; } = new List<Player>();
    }
}
