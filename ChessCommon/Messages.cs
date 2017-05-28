﻿using System;
using Newtonsoft.Json;

namespace ChessServer
{
    #region Common Types

    public class Message
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    public class MessageTypeAttribute : Attribute
    {
        public string Type { get; }

        public MessageTypeAttribute(string type)
        {
            Type = type;
        }
    }

    #endregion

    #region Join Command

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

    #endregion

    #region SendInvite Command

    [MessageType("send_invite")]
    public class SendInviteRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }
    }

    public enum SendInviteStatus
    {
        Success,
        SelfInvite,
        PlayerNotExist,
        AlreadyInvited
    }

    [MessageType("send_invite")]
    public class SendInviteResponse : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public SendInviteStatus Status { get; set; }

        public SendInviteResponse(Guid playerId, SendInviteStatus status)
        {
            PlayerId = playerId;
            Status = status;
        }
    }

    #endregion

    #region AnswerInvite Command

    public enum InviteAnswer
    {
        Accept,
        Reject
    }

    [MessageType("answer_invite")]
    public class AnswerInviteRequest : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty(PropertyName = "answer")]
        public InviteAnswer Answer { get; set; }
    }

    public enum AnswerInviteStatus
    {
        Success,
        InvalidPlayer,
        NotInvited,
    }

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

    #endregion

    #region Invite Notification

    [MessageType("invite")]
    public class InviteNotification : Message
    {
        [JsonProperty(PropertyName = "player_id")]
        public Guid PlayerId { get; set; }

        public InviteNotification(Guid playerId)
        {
            PlayerId = playerId;
        }
    }

    #endregion

    #region Move Command

    [MessageType("move")]
    public class MoveRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "move")]
        public string Move { get; set; }
    }

    public enum MoveStatus
    {
        Success,
        GameNotExist,
        NotPlayersTurn,
        InvalidMove
    }

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

    #endregion

    #region End turn command

    [MessageType("end_turn")]
    public class EndTurnRequest : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }
    }

    public enum EndTurnStatus
    {
        Success,
        GameNotExist,
        NotPlayersTurn
    }

    [MessageType("end_turn")]
    public class EndTurnResponse : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public EndTurnStatus Status { get; set; }

        public EndTurnResponse(Guid gameId, EndTurnStatus status)
        {
            GameId = gameId;
            Status = status;
        }
    }

    #endregion

    #region Move Notification

    [MessageType("move_done")]
    public class MoveNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "move")]
        public string Move { get; set; }

        public MoveNotification(Guid gameId, string move)
        {
            GameId = gameId;
            Move = move;
        }
    }

    #endregion

    #region Game notification

    [MessageType("game")]
    public class GameNotification : Message
    {
        [JsonProperty(PropertyName = "game_id")]
        public Guid GameId { get; set; }

        [JsonProperty(PropertyName = "white_player_id")]
        public Guid WhitePlayerId { get; set; }

        [JsonProperty(PropertyName = "black_player_id")]
        public Guid BlackPlayerId { get; set; }

        public GameNotification(Guid gameId, Guid whitePlayerId, Guid blackPlayerId)
        {
            GameId = gameId;
            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
        }
    }

    #endregion

    #region Player Notification

    [MessageType("player")]
    public class PlayerNotification : Message
    {
        [JsonProperty(PropertyName = "player")]
        public PlayerItem Player { get; set; }

        public PlayerNotification(PlayerItem player)
        {
            Player = player;
        }
    }

    #endregion

    #region GetPlayers Command

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
    public class GetPlayersResponse : Message
    {
        [JsonProperty(PropertyName = "players")]
        public PlayerItem[] Players { get; set; }

        public GetPlayersResponse(PlayerItem[] players)
        {
            Players = players;
        }
    }

    #endregion

    #region Other

    [MessageType("pong")]
    public class PongResponse : Message
    {
    }

    #endregion
}