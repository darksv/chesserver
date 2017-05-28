using System;
using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("end_turn")]
    public class EndTurnRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }
    }
}