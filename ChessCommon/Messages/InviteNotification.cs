using System;
using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("invite")]
    public class InviteNotification : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        public InviteNotification(Guid playerId)
        {
            PlayerId = playerId;
        }
    }
}