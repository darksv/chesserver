using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class InviteNotification : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        public InviteNotification(Guid playerId) : base("invite")
        {
            PlayerId = playerId;
        }
    }
}