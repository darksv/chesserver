using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class MoveRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "move")]
        public string Move { get; set; }

        public MoveRequest(Guid gameId, string move) : base("move")
        {
            GameId = gameId;
            Move = move;
        }
    }
}