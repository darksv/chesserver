using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class SendChatMessageResponse : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public SendMessageStatus Status { get; set; }

        [JsonConstructor]
        public SendChatMessageResponse(Guid gameId, SendMessageStatus status) : base("send_chat_message")
        {
            GameId = gameId;
            Status = status;
        }

        public SendChatMessageResponse(SendMessageStatus status) : base("send_chat_message")
        {
            GameId = Guid.Empty;
            Status = status;
        }
    }
}
