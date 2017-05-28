using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class SendInviteRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        public SendInviteRequest(Guid playerId) : base("send_invite")
        {
            PlayerId = playerId;
        }
    }
}