using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class GameNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "white_player_id")]
        public Guid WhitePlayerId { get; set; }

        [JsonProperty(PropertyName = "black_player_id")]
        public Guid BlackPlayerId { get; set; }

        public GameNotification(Guid gameId, Guid whitePlayerId, Guid blackPlayerId) : base("game")
        {
            GameId = gameId;
            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
        }
    }
}