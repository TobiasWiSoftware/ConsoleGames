using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Models.General;
using Chess.Models.Figures;



namespace Chess.Service
{
    public interface IGameService
    {
        public Game? Game { get; }
        public void SetUpGame(string p1name, string p2name);
        public void ChangePlayerOnTurn();
        public Board GetCurrentBoard();
        public Field GetFieldOfFigure(Figure figure);
        public bool SelectField(Field f);
        public bool MoveFigure(Figure figure, Field field);
        public bool MoveFigure(Rook rook);
        public bool RunGame();

    }
}
