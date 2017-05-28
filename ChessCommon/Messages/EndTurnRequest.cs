using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class EndTurnRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        public EndTurnRequest(Guid gameId) : base("end_turn")
        {
            GameId = gameId;
        }
    }
}