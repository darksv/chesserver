using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("move")]
    public class MoveResponse : Message
    {
        [JsonProperty(PropertyName = "status")]
        public MoveStatus Status { get; set; }

        public MoveResponse(MoveStatus status)
        {
            Status = status;
        }
    }
}