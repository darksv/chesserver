using System;
using Newtonsoft.Json;

namespace ChessServer
{
    public class Game
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonIgnore]
        public Player WhitePlayer { get; private set; }

        [JsonIgnore]
        public Player BlackPlayer { get; private set; }

        public Game(Player whitePlayer, Player blackPlayer)
        {
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
        }
    }
}