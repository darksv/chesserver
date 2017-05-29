using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class Move
    {
        [JsonProperty(PropertyName = "pawn_id")]
        public int PieceId { get; set; }

        [JsonProperty(PropertyName = "x")]
        public int X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public int Y { get; set; }
    }
}
