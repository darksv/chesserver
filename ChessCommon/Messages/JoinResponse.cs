using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("join")]
    public class JoinResponse : Message
    {
        [JsonProperty(PropertyName = "status")]
        public JoinStatus Status { get; set; }

        public JoinResponse(JoinStatus status)
        {
            Status = status;
        }
    }
}