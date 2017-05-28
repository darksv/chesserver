using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class PlayerNotification : Message
    {
        [JsonProperty(PropertyName = "player")]
        public Player Player { get; set; }

        public PlayerNotification(Player player) : base("player")
        {
            Player = player;
        }
    }
}