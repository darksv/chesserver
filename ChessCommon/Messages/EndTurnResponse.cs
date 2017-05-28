using System;
using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("end_turn")]
    public class EndTurnResponse : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public EndTurnStatus Status { get; set; }

        public EndTurnResponse(Guid gameId, EndTurnStatus status)
        {
            GameId = gameId;
            Status = status;
        }
    }
}