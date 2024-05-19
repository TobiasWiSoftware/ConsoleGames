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

            bool isCheck = false;

            // 1. Get the field of king of the player


            if (_board != null && _board.Fields != null)
            {
                Field kingField = (Field)(
                                            from int col in Enumerable.Range(0, _board.Fields.GetLength(0))
                                            from int row in Enumerable.Range(0, _board.Fields.GetLength(1))
                                            where _board.Fields[row, col].Figure != null && _board.Fields[row, col].Figure is King && _board.Fields[row, col].Figure != null && _board.Fields[row, col].Figure.Player == player
                                            select _board.Fields[row, col]).FirstOrDefault();

                // 2. Check Fields around the king

                // Pawn

                if (kingField.Id % 10 > 0 && kingField.Id / 10 > 0 && _board.Fields[kingField.Id % 10 - 1, kingField.Id / 10 - 1].Figure != null && _board.Fields[kingField.Id % 10 - 1, kingField.Id / 10 - 1].Figure.Player != player && _board.Fields[kingField.Id % 10 - 1, kingField.Id / 10 - 1].Figure is Pawn)
                {
                    isCheck = true;
                }

                // Rook and Queen

                if (!isCheck)
                {
                    // Two pointer with i to right and t to left
                    int i = 1;
                    int t = 1;

                    while (kingField.Id % 10 + i < _board.Fields.GetLength(1) || kingField.Id % 10 - t > -1)
                    {
                        if (kingField.Id % 10 + i < _board.Fields.GetLength(1) && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10 + i, kingField.Id / 10].Figure.Player != player && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10].Figure is Rook || _board.Fields[kingField.Id % 10 + i, kingField.Id / 10].Figure.Player != player && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                            else // If other figure is in the way
                            {
                                i = _board.Fields.GetLength(1);
                            }
                        }

                        if (kingField.Id % 10 - t > -1 && kingField.Id % 10 - t > -1 && _board.Fields[kingField.Id % 10 - t, kingField.Id / 10].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10 - t, kingField.Id / 10].Figure.Player != player && _board.Fields[kingField.Id % 10 - t, kingField.Id / 10].Figure is Rook || _board.Fields[kingField.Id % 10 - t, kingField.Id / 10].Figure.Player != player && _board.Fields[kingField.Id % 10 - t, kingField.Id / 10].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                            else
                            {
                                t = _board.Fields.GetLength(1);
                            }
                        }

                        i++;
                        t++;
                    }

                    // Same for up and down with i up and t down

                    i = 1;
                    t = 1;

                    while (kingField.Id / 10 + i < _board.Fields.GetLength(0) || kingField.Id / 10 - t > -1)
                    {
                        if (kingField.Id / 10 + i < _board.Fields.GetLength(0) && _board.Fields[kingField.Id % 10, kingField.Id / 10 + i].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10, kingField.Id / 10 + i].Figure.Player != player && _board.Fields[kingField.Id % 10, kingField.Id / 10 + i].Figure is Rook || _board.Fields[kingField.Id % 10, kingField.Id / 10 + i].Figure.Player != player && _board.Fields[kingField.Id % 10, kingField.Id / 10 + i].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                            else // If other figure is in the way
                            {
                                i = _board.Fields.GetLength(0);
                            }
                        }

                        if (kingField.Id / 10 - t > -1 && kingField.Id / 10 - t > -1 && _board.Fields[kingField.Id % 10, kingField.Id / 10 - t].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10, kingField.Id / 10 - t].Figure.Player != player && _board.Fields[kingField.Id % 10, kingField.Id / 10 - t].Figure is Rook || _board.Fields[kingField.Id % 10, kingField.Id / 10 - t].Figure.Player != player && _board.Fields[kingField.Id % 10, kingField.Id / 10 - t].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                            else
                            {
                                t = _board.Fields.GetLength(0);
                            }
                        }


                        i++;
                        t++;
                    }
                }

                // Bishop or Queen

                if (!isCheck)
                {
                    // for up right and left (array perspective)
                    int i = 1;
                    int t = 1;

                    while (kingField.Id % 10 + i < _board.Fields.GetLength(1) || kingField.Id / 10 + t < _board.Fields.GetLength(0))
                    {
                        // one to right, one up - with i to right and t to up
                        if (kingField.Id % 10 + i < _board.Fields.GetLength(1) && kingField.Id / 10 + t < _board.Fields.GetLength(0) && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 + t].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10 + i, kingField.Id / 10 + t].Figure.Player != player && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 + t].Figure is Bishop || _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 + t].Figure.Player != player && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 + t].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                        }

                        // one to left, one up with i to left and t to up
                        if (kingField.Id % 10 - i > -1 && kingField.Id / 10 + t < _board.Fields.GetLength(0) && _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 + t].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10 - i, kingField.Id / 10 + t].Figure.Player != player && _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 + t].Figure is Bishop || _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 + t].Figure.Player != player && _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 + t].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                        }

                        // one to right, one down with i to right and t to down

                        if (kingField.Id % 10 + i < _board.Fields.GetLength(1) && kingField.Id / 10 - t > -1 && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 - t].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10 + i, kingField.Id / 10 - t].Figure.Player != player && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 - t].Figure is Bishop || _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 - t].Figure.Player != player && _board.Fields[kingField.Id % 10 + i, kingField.Id / 10 - t].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                        }

                        // one to left, one down with i to left and t to down

                        if (kingField.Id % 10 - i > -1 && kingField.Id / 10 - t > -1 && _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 - t].Figure != null)
                        {
                            if (_board.Fields[kingField.Id % 10 - i, kingField.Id / 10 - t].Figure.Player != player && _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 - t].Figure is Bishop || _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 - t].Figure.Player != player && _board.Fields[kingField.Id % 10 - i, kingField.Id / 10 - t].Figure is Queen)
                            {
                                isCheck = true;
                                break;
                            }
                        }

                        i++;
                        t++;

                    }
                }

                // Knight

                if (!isCheck)
                {
                    if (kingField.Id % 10 + 2 < _board.Fields.GetLength(0) && kingField.Id / 10 + 1 < _board.Fields.GetLength(1) && _board.Fields[kingField.Id % 10 + 2, kingField.Id / 10 + 1].Figure != null && _board.Fields[kingField.Id % 10 + 2, kingField.Id / 10 + 1].Figure.Player != player && _board.Fields[kingField.Id % 10 + 2, kingField.Id / 10 + 1].Figure is Knight)
                    {
                        isCheck = true;
                    }
                    else if (kingField.Id % 10 + 1 < _board.Fields.GetLength(0) && kingField.Id / 10 + 2 < _board.Fields.GetLength(1) && _board.Fields[kingField.Id % 10 + 1, kingField.Id / 10 + 2].Figure != null && _board.Fields[kingField.Id % 10 + 1, kingField.Id / 10 + 2].Figure.Player != player && _board.Fields[kingField.Id % 10 + 1, kingField.Id / 10 + 2].Figure is Knight)
                    {
                        isCheck = true;
                    }
                    else if (kingField.Id % 10 - 1 > 0 && kingField.Id / 10 + 2 < _board.Fields.GetLength(1) && _board.Fields[kingField.Id % 10 - 1, kingField.Id / 10 + 2].Figure != null && _board.Fields[kingField.Id % 10 - 1, kingField.Id / 10 + 2].Figure.Player != player && _board.Fields[kingField.Id % 10 - 1, kingField.Id / 10 + 2].Figure is Knight)
                    {
                        isCheck = true;
                    }
                    else if (kingField.Id % 10 - 2 > 0 && kingField.Id / 10 + 1 < _board.Fields.GetLength(1) && _board.Fields[kingField.Id % 10 - 2, kingField.Id / 10 + 1].Figure != null && _board.Fields[kingField.Id % 10 - 2, kingField.Id / 10 + 1].Figure.Player != player && _board.Fields[kingField.Id % 10 - 2, kingField.Id / 10 - 2].Figure is Knight)
                    {
                        isCheck = true;
                    }
                }

            }


            return isCheck;

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
            if (game.Player2 != game.PlayerOnTurn)
            {
                ReverseBoardFields();
            }

            try
            {
                // 1. check if the move is possible without checking check, checkmate and so on
                bool isPossible = _figureService.ValidateMovePossibility(game, this, GetFieldOfFigure(figure), destinationField);

                // 2. check own check

                if (game.PlayerOnTurn.IsCheck)
                {
                    // save previous state

                    Field oldField = GetFieldOfFigure(figure);

                    // move and see if check is away

                    destinationField.Figure = figure;
                    oldField.Figure = null;

                    game.PlayerOnTurn.IsCheck = IsCheck(game.PlayerOnTurn);

                    if (game.PlayerOnTurn.IsCheck)
                    {
                        // return to previous state bec. check is still there
                        destinationField.Figure = null;
                        oldField.Figure = figure;
                        throw new Exception("Move not possible, check is still there");
                    }
                    else
                    {
                        isPossible = true;
                    }

                }


                if (isPossible)
                {
                    GetFieldOfFigure(figure).Figure = null;
                    destinationField.Figure = figure;

                    if (figure is Pawn pawn)
                    {
                        if (pawn.TwoStepPossible)
                        {
                            pawn.TwoStepPossible = false;
                             pawn.EnPassantePossible = true; // It can be thrown via en passant
                        }
                        else
                        {
                            pawn.EnPassantePossible = false;
                        }
                    }

                    if (figure is Pawn)
                    {
                        // Edge case for throwing via en passant

                        // Get the opp pawn and check if it is possible to throw it
                        figure = _board.Fields[destinationField.Id % 10, destinationField.Id / 10 - 1].Figure;
                    }


                    switch (figure)
                    {
                        case Pawn oPawn when oPawn.EnPassantePossible:

                            // Edge case for en passante
                            _board.Fields[destinationField.Id % 10, destinationField.Id / 10 - 1].Figure = null;
                            break;
                        case Rook rook:
                            ((Rook)figure).IsFirstMove = false;
                            break;
                        default:
                            break;
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
                if (game.Player2 != game.PlayerOnTurn)
                {
                    ReverseBoardFields();
                }
            }
        }
        public bool MoveFigure(Game game, Rook rook)
        {
            // Pull king and destination filed
            // 
            King? king = _board.Fields[4,0].Figure as King;
            Field destinationField = game.Board.Fields[rook.IsKingRook ? 5 : 3, GetFieldOfFigure(king).Id / 10];
            
            if (game.Player2 != game.PlayerOnTurn)
            {
                ReverseBoardFields();
                king = _board.Fields[3,0].Figure as King;
                destinationField = game.Board.Fields[rook.IsKingRook ? 2 : 4, GetFieldOfFigure(king).Id / 10];
            }





            try
            {
                // 1. check if the move is possible without checking check, checkmate and so on
                bool isPossible = _figureService.ValidateMovePossibility(game, this, GetFieldOfFigure(rook), destinationField);

                // 2. check own check
                if (isPossible)
                {
                    // It is the edge case for castling

                    GetFieldOfFigure(king).Figure = null;

                    if (rook.IsKingRook) // Short casteling move
                    {
                        game.Board.Fields[destinationField.Id % 10 - 1, destinationField.Id / 10].Figure = rook;
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
                if (game.Player2 != game.PlayerOnTurn)
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
