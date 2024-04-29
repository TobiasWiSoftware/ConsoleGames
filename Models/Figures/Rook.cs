using Chess.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.Figures
{
    public class Rook : Figure
    {
        public bool IsKingRook { get; set; } // Only two cases possible - next to queen or rook - important for castling
        public bool IsFirstMove { get; set; } // For castling
        public Rook(int id, Player player, bool isKingRook) : base(id, player)
        {
            IsKingRook = isKingRook;
            IsFirstMove = true;
        }
    }
}
