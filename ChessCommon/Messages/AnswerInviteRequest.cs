using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    public class AnswerInviteRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "answer")]
        public InviteAnswer Answer { get; set; }

        public AnswerInviteRequest(Guid playerId, InviteAnswer answer) : base("answer_invite")
        {
            PlayerId = playerId;
            Answer = answer;
        }
    }
}