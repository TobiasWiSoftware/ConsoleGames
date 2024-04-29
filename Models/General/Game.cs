using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.General
{
    public class Game
    {
        // Singelton
        private static Game? _game = null;
        public Player Player1 { get => _player1; }
        public Player Player2 { get => _player2; }
        public Player PlayerOnTurn { get; set; }
        public Board Board { get => _board; }
        // Singelton
        private Board _board;

        private Player _player1;

        private Player _player2;
        public bool IsActive { get => Player1.IsCheckmate == false && Player2.IsCheckmate == false; }
        private Game(Player player1, Player player2) // Singelton
        {
            _player1 = player1;
            _player2 = player2;
            _board = Board.getTheBoard(player1, player2);
            PlayerOnTurn = Player1;
        }
        public static Game getTheGame(Player player1, Player player2) => _game ??= new(player1, player2);

    }
}
