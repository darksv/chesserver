using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class GetPlayersResponse : Message
    {
        [JsonProperty(PropertyName = "players")]
        public Player[] Players { get; set; }

        public GetPlayersResponse(Player[] players) : base("players")
        {
            Players = players;
        }
    }
}