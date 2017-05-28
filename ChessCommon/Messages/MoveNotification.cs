using System;
using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("move_done")]
    public class MoveNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "move")]
        public string Move { get; set; }

        public MoveNotification(Guid gameId, string move)
        {
            GameId = gameId;
            Move = move;
        }
    }
}