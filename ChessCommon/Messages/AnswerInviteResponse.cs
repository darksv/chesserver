using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class AnswerInviteResponse : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public AnswerInviteStatus Status { get; set; }

        public AnswerInviteResponse(Guid playerId, AnswerInviteStatus status) : base("answer_invite")
        {
            PlayerId = playerId;
            Status = status;
        }
    }
}