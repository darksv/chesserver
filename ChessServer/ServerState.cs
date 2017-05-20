using System.Collections.Generic;

namespace ChessServer
{
    public class ServerState
    {
        private readonly List<Game> _games = new List<Game>();
        private readonly List<Player> _players = new List<Player>();
    }
}
