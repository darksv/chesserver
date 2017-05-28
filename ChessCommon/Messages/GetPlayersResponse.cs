using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("players")]
    public class GetPlayersResponse : Message
    {
        [JsonProperty(PropertyName = "players")]
        public Player[] Players { get; set; }

        public GetPlayersResponse(Player[] players)
        {
            Players = players;
        }
    }
}