using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class EndTurnResponse : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public EndTurnStatus Status { get; set; }

        public EndTurnResponse(Guid gameId, EndTurnStatus status) : base("end_turn")
        {
            GameId = gameId;
            Status = status;
        }
    }
}