using System;
using Newtonsoft.Json;

namespace ChessServer
{
    public class Message
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    public class MessageTypeAttribute : Attribute
    {
        public string Type { get; private set; }

        public MessageTypeAttribute(string type)
        {
            Type = type;
        }
    }

    [MessageType("pong")]
    public class PongResponse : Message
    {
    }

    public class PlayerItem
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }

        [JsonProperty(PropertyName = "status")]
        public ClientStatus Status { get; set; }
    }

    [MessageType("players")]
    public class OnlinePlayers : Message
    {
        [JsonProperty(PropertyName = "players")]
        public PlayerItem[] Players { get; set; }
    }

    [MessageType("join")]
    public class JoinRequest : Message
    {
        [JsonProperty(PropertyName = "nick")]
        public string Nick { get; set; }
    }

    public enum JoinStatus
    {
        Success,
        NickInvalid,
        NickOccupied,
        AlreadyJoined
    }

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

    public enum InviteSendStatus
    {
        Success,
        SelfInvite,
        PlayerNotExist,
        AlreadyInvited
    }

    [MessageType("send_invite")]
    public class InviteSendRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }
    }

    [MessageType("send_invite")]
    public class InviteSendResponse : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public InviteSendStatus Status { get; set; }

        public InviteSendResponse(Guid playerId, InviteSendStatus status)
        {
            PlayerId = playerId;
            Status = status;
        }
    }

    [MessageType("invite")]
    public class InviteMessage : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        public InviteMessage(Guid playerId)
        {
            PlayerId = playerId;
        }
    }

    public enum InviteAnswer
    {
        Accept,
        Reject
    }

    [MessageType("answer_invite")]
    public class InviteAnswerRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "answer")]
        public InviteAnswer Answer { get; set; }
    }

    public enum InviteAnswerStatus
    {
        Success,
        InvalidPlayer,
        NotInvited,
    }

    [MessageType("answer_invite")]
    public class InviteAnswerResponse : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public InviteAnswerStatus Status { get; set; }

        public InviteAnswerResponse(Guid playerId, InviteAnswerStatus status)
        {
            PlayerId = playerId;
            Status = status;
        }
    }
}
