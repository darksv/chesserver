using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("move")]
    public class MoveRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "move")]
        public string Move { get; set; }
    }
}