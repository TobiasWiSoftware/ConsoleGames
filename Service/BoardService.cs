using Chess.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Chess.Models.Figures;


namespace Chess.Service
{
    public class BoardService : IBoardService
    {
        public Board? Board { get => _board; }
        private Board? _board;
        private readonly IFigureService _figureService;
        public BoardService(IFigureService figureService)
        {
            _figureService = figureService;
        }
        public void SetUpBoard(Player p1, Player p2)
        {
            _board = Board.getTheBoard(p1, p2);
        }
        public Field GetFieldOfFigure(Figure figure)
        {
            Field? field = (from int row in Enumerable.Range(0, _board.Fields.GetLength(0))
                            from int col in Enumerable.Range(0, _board.Fields.GetLength(1))
                            where _board.Fields[row, col].Figure == figure
                            select _board.Fields[row, col])
                            .FirstOrDefault();

            if (field == null)
            {
                throw new Exception("Figure not found");
            }

            return field;
        }
        public bool AddFigure(Figure figureOld, Figure figureNew)
        {
            throw new NotImplementedException();
        }
        public bool IsCheck(Player player)
        {
            throw new NotImplementedException();
        }
        public bool IsCheckAfterMove(int figureId, Field field)
        {
            throw new NotImplementedException();
        }
        public bool IsCheckmate(Player player)
        {
            throw new NotImplementedException();
        }
        public bool ValidateFieldPossible(Field emtyField)
        {
            if (emtyField.Figure == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ValidateFieldPossible(Game game, Field destinationField, Field figureField)
        {
            bool isPossible = false;

            // Check if figure can run that way when field emty

            if (destinationField.Figure == null || destinationField.Figure.Player != game.PlayerOnTurn)
            {
                isPossible = _figureService.ValidateMovePossibility(game, this, figureField, destinationField);
            }
            else
            {
                throw new Exception("Field is blocked by own figure");
            }

            return isPossible;
        }
        public void ReverseBoardFields()
        {
            Field[,] fields = new Field[0, 0];

            if (_board != null)
            {
                fields = new Field[_board.Fields.GetLength(0), _board.Fields.GetLength(1)];

                for (int row = _board.Fields.GetLength(1) - 1; row > -1; row--)
                {
                    for (int col = _board.Fields.GetLength(0) - 1; col > -1; col--)
                    {
                        int oldCol = _board.Fields.GetLength(0) - col - 1;
                        int oldRow = _board.Fields.GetLength(1) - row - 1;

                        fields[col, row] = _board.Fields[oldCol, oldRow];
                        fields[col, row].Figure = _board.Fields[oldCol, oldRow].Figure;
                        fields[col, row].Id = _board.Fields.GetLength(0) - 1 - _board.Fields[oldCol, oldRow].Id % 10 + (_board.Fields.GetLength(1) - 1 - _board.Fields[oldCol, oldRow].Id / 10) * 10;
                    }
                }
                _board.Fields = fields;
            }

        }
        public bool MoveFigure(Game game, Figure figure, Field destinationField)
        {
            if (game.Player1 != game.PlayerOnTurn)
            {
                ReverseBoardFields();
            }

            try
            {
                // 1. check if the move is possible without checking check, checkmate and so on
                bool isPossible = _figureService.ValidateMovePossibility(game, this, GetFieldOfFigure(figure), destinationField);

                // 2. check own check


                if (isPossible)
                {
                    GetFieldOfFigure(figure).Figure = null;



                    switch (figure)
                    {
                        case Pawn pawn when pawn.IsFirstMove || pawn.IsSecondMove:
                            // Edge case for throwing via en passant
                            if (destinationField.Figure == null && _board != null)
                            {
                                _board.Fields[destinationField.Id % 10, destinationField.Id / 10 - 1].Figure = null;
                            }
                            if (pawn.IsFirstMove)
                            {
                                pawn.IsFirstMove = false;
                                pawn.IsSecondMove = true;
                            }
                            else
                            {
                                pawn.IsSecondMove = false;
                            }
                            break;
                        case Rook rook:
                            ((Rook)figure).IsFirstMove = false;
                            break;
                        default:
                            break;
                    }

                    destinationField.Figure = figure;

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (game.Player1 != game.PlayerOnTurn)
                {
                    ReverseBoardFields();
                }
            }
        }
        public bool MoveFigure(Game game, King king, Rook rook)
        {
            if (game.Player1 != game.PlayerOnTurn)
            {
                ReverseBoardFields();
            }

            Field destinationField = game.Board.Fields[rook.IsKingRook ? 6 : 2, GetFieldOfFigure(king).Id / 10];

            try
            {
                // 1. check if the move is possible without checking check, checkmate and so on
                bool isPossible = _figureService.ValidateMovePossibility(game, this, GetFieldOfFigure(king), destinationField );

                // 2. check own check
                if (isPossible)
                {
                    // It is the edge case for castling

                    GetFieldOfFigure(king).Figure = null;

                    if(rook.IsKingRook) // Short casteling move
                    {
                        game.Board.Fields[destinationField.Id % 10  - 1, destinationField.Id / 10].Figure = rook;
                    }
                    else // Long casteling move
                    {
                        game.Board.Fields[destinationField.Id % 10 + 1, destinationField.Id / 10].Figure = rook;
                    }

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (game.Player1 != game.PlayerOnTurn)
                {
                    ReverseBoardFields();
                }
            }
        }
        public bool SelectFigure(Field field)
        {
            throw new NotImplementedException();
        }
    }
}
