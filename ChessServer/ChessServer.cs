using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Common;
using Chess.Common.Messages;
using Newtonsoft.Json;

namespace Chess.Server
{
    public class ChessServer
    {
        private readonly Action<object> _logger;
        private readonly Server _server = new Server();
        private readonly object _lock = new object();
        private readonly List<Client> _clients = new List<Client>();
        private readonly Dictionary<string, Action<Client, string>> _handlers;
        private readonly List<Game> _games = new List<Game>();
        private readonly List<Invitation> _invitations = new List<Invitation>();

        public ChessServer(Action<object> logger)
        {
            _logger = logger;
            _handlers = new Dictionary<string, Action<Client, string>>
            {
                {"join", HandleJoin},
                {"leave", HandleLeave},
                {"send_invite", HandleSendInvite},
                {"answer_invite", HandleAnswerInvite},
                {"get_players", HandleGetPlayers},
                {"move", HandleMove},
                {"promote", HandlePromote},
                {"end_turn", HandleEndTurn},
                {"send_chat_message", HandleSendChatMessage}
            };
        }

        public void Run(ushort port)
        {
            _server.Receive += ServerOnReceive;
            _server.Connect += ServerOnConnect;
            _server.Disconnect += ServerOnDisconnect;
            _server.Start(port);
        }

        private void Log(object value)
        {
            _logger.Invoke(value);
        }

        private void ServerOnDisconnect(object sender, ConnectionEventArgs args)
        {
            lock (_lock)
            {
                var client = _clients.FirstOrDefault(p => p.Socket == args.ClientSocket);
                if (client != null)
                {
                    client.Status = PlayerStatus.Disconnected;
                }
            }

            Log($"We've lost connection with {args.ClientSocket.RemoteEndPoint} (Reason: {args.DisconnectionReason})");
        }

        private void ServerOnConnect(object sender, ConnectionEventArgs args)
        {
            var client = new Client(args.ClientSocket);
            lock (_lock)
            {
                _clients.Add(client);
            }

            NotifyClientChange(client);

            Log($"We've established connection with {args.ClientSocket.RemoteEndPoint}");
        }

        private void ServerOnReceive(object sender, ReceiveEventArgs args)
        {
            Client client;
            lock (_lock)
            {
                client = _clients.FirstOrDefault(p => p.Socket == args.ClientSocket);
            }

            var messageType = MessageHelpers.ExtractType(args.Message);
            if (messageType != null && _handlers.TryGetValue(messageType, out var handler))
            {
                handler(client, args.Message);
                Log($"Handled {messageType} command");
            }
            else
            {
                Log("Error: unknown command");
            }
        }

        #region Message Handlers
        
        private void HandleJoin(Client client, string data)
        {
            if (client.Status != PlayerStatus.Connected)
            {
                Send(client, new JoinResponse(JoinStatus.AlreadyJoined));
                return;
            }

            var nick = JsonConvert.DeserializeObject<JoinRequest>(data).Nick;

            if (string.IsNullOrWhiteSpace(nick))
            {
                Send(client, new JoinResponse(JoinStatus.NickInvalid));
                return;
            }

            if (IsNickTaken(nick))
            {
                Send(client, new JoinResponse(JoinStatus.NickOccupied));
                return;
            }

            client.Nick = nick;
            client.Status = PlayerStatus.Joined;
            Send(client, new JoinResponse(JoinStatus.Success));
            NotifyClientChange(client);

            Log($"Client {client.Socket.RemoteEndPoint} joined to the server as {nick}");
        }

        private void HandleLeave(Client client, string args)
        {
            client.Status = PlayerStatus.Left;
            NotifyClientChange(client);

            Log($"{client.Nick} has left the game");
        }

        private void HandleSendInvite(Client client, string data)
        {
            var clientId = JsonConvert.DeserializeObject<SendInviteRequest>(data).PlayerId;
            if (clientId == client.Id)
            {
                Send(client, new SendInviteResponse(clientId, SendInviteStatus.SelfInvite));
                return;
            }

            Client invitedClient;
            lock (_lock)
            {
                invitedClient = _clients.FirstOrDefault(c => c.Id == clientId);
            }

            if (invitedClient == null)
            {
                Send(client, new SendInviteResponse(clientId, SendInviteStatus.PlayerNotExist));
                return;
            }

            if (_invitations.Any(i => i.InvitedPlayer == invitedClient && i.InvitingPlayer == client && !i.Answer.HasValue))
            {
                Send(client, new SendInviteResponse(clientId, SendInviteStatus.AlreadyInvited));
                return;
            }

            _invitations.Add(new Invitation
            {
                InvitingPlayer = client,
                InvitedPlayer = invitedClient,
            });

            Send(client, new SendInviteResponse(clientId, SendInviteStatus.Success));
            Send(invitedClient, new InviteNotification(client.Id));

            Log($"{client.Nick} has invited {invitedClient.Nick} to the game");
        }

        private void HandleAnswerInvite(Client client, string data)
        {
            var request = JsonConvert.DeserializeObject<AnswerInviteRequest>(data);
            if (request.PlayerId == client.Id)
            {
                Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.InvalidPlayer));
                return;
            }

            Client invitingClient;
            lock (_lock)
            {
                invitingClient = _clients.FirstOrDefault(c => c.Id == request.PlayerId);
            }

            if (invitingClient == null)
            {
                Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.InvalidPlayer));
                return;
            }

            if (!_invitations.Any(i => i.InvitedPlayer == client && i.InvitingPlayer == invitingClient &&
                                       !i.Answer.HasValue))
            {
                Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.NotInvited));
                return;
            }

            if (request.Answer == InviteAnswer.Accept)
            {
                var game = new Game(client, invitingClient);
                client.Game = game;
                client.Status = PlayerStatus.OnGame;
                NotifyClientChange(client);

                invitingClient.Game = game;
                invitingClient.Status = PlayerStatus.OnGame;
                NotifyClientChange(invitingClient);

                _games.Add(game);

                var notification = new GameNotification(game.Id, game.WhitePlayer.Id, game.BlackPlayer.Id);
                Send(invitingClient, notification);
                Send(client, notification);
            }
            else
            {
                Log($"{client.Nick} has rejected {invitingClient.Nick}'s invitation");
            }

            Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.Success));
        }

        private void HandleGetPlayers(Client client, string data)
        {
            Player[] players;
            lock (_lock)
            {
                players = _clients
                    .Where(c => c.Status == PlayerStatus.Joined)
                    .Select(c => new Player
                    {
                        Id = c.Id,
                        Nick = c.Nick,
                        Status = c.Status
                    })
                    .ToArray();
            }

            Send(client, new GetPlayersResponse(players));
        }

        private void HandleMove(Client client, string data)
        {
            var request = JsonConvert.DeserializeObject<MoveRequest>(data);

            var game = _games.FirstOrDefault(g => g.Id == request.GameId);
            if (game == null)
            {
                Send(client, new MoveResponse(MoveStatus.GameNotExist));
                return;
            }

            if (!game.CanDoMove(client))
            {
                Send(client, new MoveResponse(MoveStatus.NotPlayersTurn));
                return;
            }

            // TODO: check move

            Send(client, new MoveResponse(MoveStatus.Success));
            Send(game.GetOpponentFor(client), new MoveNotification(game.Id, request.Move));
        }

        private void HandlePromote(Client client, string data)
        {
            var request = JsonConvert.DeserializeObject<PromoteRequest>(data);

            var game = _games.FirstOrDefault(g => g.Id == request.GameId);
            if (game == null)
            {
                Send(client, new PromoteResponse());
                return;
            }

            if (!game.CanDoMove(client))
            {
                Send(client, new PromoteResponse(game.Id));
                return;
            }

            // TODO: check promotion

            Send(client, new PromoteResponse());
            Send(game.GetOpponentFor(client), new PromoteNotification(game.Id, request.PieceId, request.PromoteTo));
        }

        private void HandleEndTurn(Client client, string data)
        {
            var request = JsonConvert.DeserializeObject<EndTurnRequest>(data);

            var game = _games.FirstOrDefault(g => g.Id == request.GameId);
            if (game == null)
            {
                Send(client, new EndTurnResponse(request.GameId, EndTurnStatus.GameNotExist));
                return;
            }

            if (!game.CanDoMove(client))
            {
                Send(client, new EndTurnResponse(game.Id, EndTurnStatus.NotPlayersTurn));
                return;
            }

            game.SwitchTurn();
            Send(client, new EndTurnResponse(game.Id, EndTurnStatus.Success));
            Send(game.GetOpponentFor(client), new EndTurnNotification(game.Id));
        }

        private void HandleSendChatMessage(Client client, string data)
        {
            var request = JsonConvert.DeserializeObject<SendChatMessageRequest>(data);

            var notification = new ChatMessageNotification(client.Id, request.GameId, request.Message);
            if (request.GameId == Guid.Empty)
            {
                foreach (var clientToNotify in _clients)
                {
                    Send(clientToNotify, notification);
                }
            }
            else
            {
                var game = _games.FirstOrDefault(g => g.InvolvesPlayer(client));
                if (game == null)
                {
                    Send(client, new SendChatMessageResponse(SendMessageStatus.Error));
                    return;
                }

                var opponent = game.GetOpponentFor(client);
                Send(opponent, notification);
            }

            Send(client, new SendChatMessageResponse(SendMessageStatus.Success));
        }

        #endregion

        private void Send<T>(Client client, T message)
            where T : Message
        {
            _server.Send(client.Socket, MessageHelpers.SerializeMessage(message));
        }

        private bool IsNickTaken(string nick)
        {
            lock (_lock)
            {
                return _clients.Any(p => p.Status != PlayerStatus.Left && p.Status != PlayerStatus.Disconnected && p.Nick == nick);
            }
        }

        private void NotifyClientChange(Client client)
        {
            var notification = new PlayerNotification(new Player
            {
                Id = client.Id,
                Nick = client.Nick,
                Status = client.Status
            });

            Client[] clientsToNotify;
            lock (_lock)
            {
                clientsToNotify = _clients
                    .Where(c => c.Status == PlayerStatus.Joined || c.Status == PlayerStatus.OnGame)
                    .Where(c => c != client)
                    .ToArray();
            }

            foreach (var otherClient in clientsToNotify)
            {
                Send(otherClient, notification);
            }
        }
    }
}