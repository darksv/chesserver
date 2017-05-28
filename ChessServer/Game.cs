namespace ChessServer
{
    public enum Color
    {
        Black,
        White
    }

    public class Game
    {
        public Client WhitePlayer { get; }
        public Client BlackPlayer { get; }
        public Color CurrentTurn { get; set; }

        public Game(Client whitePlayer, Client blackPlayer)
        {
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
        }

        public bool InvolvesPlayer(Client player)
        {
            return WhitePlayer == player || BlackPlayer == player;
        }

        public Client GetOpponentFor(Client player)
        {
            return WhitePlayer == player 
                ? BlackPlayer 
                : (BlackPlayer == player ? WhitePlayer : null);
        }

        public bool CanDoMove(Client player)
        {
            return WhitePlayer == player && CurrentTurn == Color.White
                || BlackPlayer == player && CurrentTurn == Color.Black;
        }

        public void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == Color.Black ? Color.White : Color.Black;
        }
    }
}