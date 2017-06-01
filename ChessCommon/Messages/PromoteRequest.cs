using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class PromoteRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "piece_id")]
        public int PieceId { get; set; }

        [JsonProperty(PropertyName = "promote_to")]
        public int PromoteTo { get; set; }

        public PromoteRequest(Guid gameId, int pieceId, int promoteTo) : base("promote")
        {
            GameId = gameId;
            PieceId = pieceId;
            PromoteTo = promoteTo;
        }
    }
}