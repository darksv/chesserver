﻿using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class SendChatMessageRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonConstructor]
        public SendChatMessageRequest(Guid gameId, string message) : base("send_chat_message")
        {
            GameId = gameId;
            Message = message;
        }

        public SendChatMessageRequest(string message) : base("send_chat_message")
        {
            GameId = Guid.Empty;
            Message = message;
        }
    }
}
