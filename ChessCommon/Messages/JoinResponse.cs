using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class JoinResponse : Message
    {
        [JsonProperty(PropertyName = "status")]
        public JoinStatus Status { get; set; }

        public JoinResponse(JoinStatus status) : base("join")
        {
            Status = status;
        }
    }
}