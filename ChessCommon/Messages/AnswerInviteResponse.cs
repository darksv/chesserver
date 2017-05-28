using System;
using Newtonsoft.Json;

namespace ChessServer
{
    [MessageType("answer_invite")]
    public class AnswerInviteResponse : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public AnswerInviteStatus Status { get; set; }

        public AnswerInviteResponse(Guid playerId, AnswerInviteStatus status)
        {
            PlayerId = playerId;
            Status = status;
        }
    }
}