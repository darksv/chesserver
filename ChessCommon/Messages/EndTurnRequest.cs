using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("end_turn")]
    public class EndTurnRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }
    }
}