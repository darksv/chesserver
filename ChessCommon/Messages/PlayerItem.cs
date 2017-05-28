using System;
using Newtonsoft.Json;

namespace ChessServer
{
    public class PlayerItem
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }

        [JsonProperty(PropertyName = "status")]
        public ClientStatus Status { get; set; }
    }
}