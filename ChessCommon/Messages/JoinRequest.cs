using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class JoinRequest : Message
    {
        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }

        public JoinRequest(string nick) : base("join")
        {
            Nick = nick;
        }
    }
}