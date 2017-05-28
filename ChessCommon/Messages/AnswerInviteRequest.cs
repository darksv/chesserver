using System;
using Newtonsoft.Json;

namespace Chess.Common.Messages
{
    [MessageType("answer_invite")]
    public class AnswerInviteRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "answer")]
        public InviteAnswer Answer { get; set; }
    }
}