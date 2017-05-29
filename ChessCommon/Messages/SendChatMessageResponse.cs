using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class SendChatMessageResponse : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; }

        [JsonProperty(PropertyName = "status")]
        public SendMessageStatus Status { get; }

        public SendChatMessageResponse(Guid gameId) : base("send_chat_message")
        {
            GameId = gameId;
        }

        public SendChatMessageResponse() : base("send_chat_message")
        {
            GameId = Guid.Empty;
        }
    }
}
