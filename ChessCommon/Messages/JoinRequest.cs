using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("join")]
    public class JoinRequest : Message
    {
        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }
    }
}