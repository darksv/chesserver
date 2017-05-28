using Newtonsoft.Json;

namespace Chess.Common.Messages
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