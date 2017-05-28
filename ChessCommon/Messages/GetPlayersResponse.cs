using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("players")]
    public class GetPlayersResponse : Message
    {
        [JsonProperty(PropertyName = "players")]
        public PlayerItem[] Players { get; set; }

        public GetPlayersResponse(PlayerItem[] players)
        {
            Players = players;
        }
    }
}