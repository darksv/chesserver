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

    public class PongResponse : Message
    {
        public PongResponse()
        {
            Type = "pong";
        }
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

    [MessageType("online_players")]
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
        NickExists,
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

    public enum InviteStatus
    {
        Success,
        SelfInvite,
        PlayerNotExist,
    }

    [MessageType("invite")]
    public class InviteSendRequest : Message
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
    }

    [MessageType("invite")]
    public class InviteReceiveRequest : Message
    {
        [JsonProperty(PropertyName = "player")]
        public PlayerItem Player { get; set; }
    }

    [MessageType("invite")]
    public class InviteSendResponse : Message
    {
        [JsonProperty(PropertyName = "status")]
        public InviteStatus Status { get; set; }

        public InviteSendResponse(InviteStatus status)
        {
            Status = status;
        }
    }
}
