using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
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