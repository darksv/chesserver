using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("send_invite")]
    public class SendInviteResponse : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public SendInviteStatus Status { get; set; }

        public SendInviteResponse(Guid playerId, SendInviteStatus status)
        {
            PlayerId = playerId;
            Status = status;
        }
    }
}