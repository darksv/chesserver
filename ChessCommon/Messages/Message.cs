using Newtonsoft.Json;

namespace ChessServer
{
    public class Message
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}