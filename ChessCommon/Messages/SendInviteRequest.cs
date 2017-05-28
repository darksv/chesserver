using System;
using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("send_invite")]
    public class SendInviteRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }
    }
}