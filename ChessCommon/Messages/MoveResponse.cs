using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class MoveResponse : Message
    {
        [JsonProperty(PropertyName = "status")]
        public MoveStatus Status { get; set; }

        public MoveResponse(MoveStatus status) : base("move")
        {
            Status = status;
        }
    }
}