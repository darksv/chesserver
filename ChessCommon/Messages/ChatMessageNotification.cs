using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class ChatMessageNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Message { get; set; }

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
