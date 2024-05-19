using Chess.Models.Figures;
using Chess.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Service
{
    public interface IBoardService
    {
        public Board? Board { get; }

        public void SetUpBoard(Player p1, Player p2);
        public Field GetFieldOfFigure(Figure figure);
        public bool SelectFigure(Field field);
        public bool ValidateFieldPossible(Field emtyfield);
        public bool ValidateFieldPossible(Game game, Field destinationField, Field figureField);
        public void ReverseBoardFields();
        public bool MoveFigure(Game game, Figure figure, Field field);
        public bool MoveFigure(Game game, Rook rook);
        public bool IsCheck(Player player);
        public bool IsCheckAfterMove(int figureId, Field field);
        public bool IsCheckmate(Player player);
        public bool AddFigure(Figure figureOld, Figure figureNew);
    }
}
