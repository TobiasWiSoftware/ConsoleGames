using Chess.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.Figures
{
    public class Pawn : Figure
    {
        public bool TwoStepPossible { get; set; } = true;   
        public bool EnPassantePossible { get; set; } = false;
        public Pawn(int id, Player player) : base(id, player)
        {
        }
    }
}
