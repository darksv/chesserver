using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("join")]
    public class JoinRequest : Message
    {
        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }
    }
}