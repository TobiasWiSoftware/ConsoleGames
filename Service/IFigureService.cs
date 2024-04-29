using Chess.Models.Figures;
using Chess.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Service
{
    public interface IFigureService
    {
        public bool ValidateMovePossibility(Game game, IBoardService boardService, Field Fieldfigure, Field destinationField);
        public bool ValidateEdgeCaseForPawn(Game game, IBoardService boardService, Field pawnField, Field destinationField);
        public bool ChangeFigure(Pawn pawn, Figure figure);
    }
}
