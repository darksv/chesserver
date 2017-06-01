using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class EndTurnNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        public EndTurnNotification(Guid gameId) : base("ended_turn")
        {
            GameId = gameId;
        }
    }
}
