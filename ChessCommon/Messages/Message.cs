using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class Message
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        public Message(string type)
        {
            Type = type;
        }
    }
}