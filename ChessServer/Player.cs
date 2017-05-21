using System;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace ChessServer
{
    public class Player
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }

        [JsonIgnore]
        public Socket ClientSocket { get; set; }
    }
}
