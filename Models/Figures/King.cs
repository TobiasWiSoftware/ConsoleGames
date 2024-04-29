using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models.General;

namespace Chess.Models.Figures
{
    public class King : Figure
    {
        public bool IsFirstMove { get; set; }
        public King(int id, Player player) : base(id, player)
        {
            IsFirstMove = true;
        }
    }
}
