using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("player")]
    public class PlayerNotification : Message
    {
        [JsonProperty(PropertyName = "player")]
        public Player Player { get; set; }

        public PlayerNotification(Player player)
        {
            Player = player;
        }
    }
}