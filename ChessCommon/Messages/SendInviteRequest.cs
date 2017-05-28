using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("send_invite")]
    public class SendInviteRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }
    }
}