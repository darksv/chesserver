using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("player")]
    public class PlayerNotification : Message
    {
        [JsonProperty(PropertyName = "player")]
        public PlayerItem Player { get; set; }

        public PlayerNotification(PlayerItem player)
        {
            Player = player;
        }
    }
}