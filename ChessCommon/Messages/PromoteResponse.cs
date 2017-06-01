using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class PromoteResponse : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        public PromoteResponse(Guid gameId) : base("promote")
        {
            GameId = gameId;
        }

        public PromoteResponse() : base("promote")
        {
            GameId = Guid.Empty;
        }
    }
}
