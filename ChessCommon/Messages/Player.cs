using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class Player
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }

        [JsonProperty(PropertyName = "status")]
        public PlayerStatus Status { get; set; }
    }
}