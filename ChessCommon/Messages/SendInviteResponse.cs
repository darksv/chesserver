using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class SendInviteResponse : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public SendInviteStatus Status { get; set; }

        public SendInviteResponse(Guid playerId, SendInviteStatus status) : base("send_invite")
        {
            PlayerId = playerId;
            Status = status;
        }
    }
}