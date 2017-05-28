using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class MoveNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "move")]
        public string Move { get; set; }

        public MoveNotification(Guid gameId, string move) : base("move_done")
        {
            GameId = gameId;
            Move = move;
        }
    }
}