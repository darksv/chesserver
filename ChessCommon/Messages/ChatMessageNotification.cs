using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class ChatMessageNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; }

        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; }

        [JsonProperty(PropertyName = "status")]
        public string Message { get; }

        public ChatMessageNotification(Guid playerId, string message) : base("chat_message")
        {
            PlayerId = playerId;
            GameId = Guid.Empty;
            Message = message;
        }

        public ChatMessageNotification(Guid playerId, Guid gameId, string message) : base("chat_message")
        {
            PlayerId = playerId;
            GameId = gameId;
            Message = message;
        }
    }
}
