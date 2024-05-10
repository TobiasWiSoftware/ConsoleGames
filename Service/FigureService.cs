using Chess.Models.Figures;
using Chess.Models.General;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chess.Service
{
    public class FigureService : IFigureService
    {
        public bool ValidateMovePossibility(Game game, IBoardService boardService, Field fieldFigure, Field destinationField)
        {
            bool result = false;
            Field[,] fields = game.Board.Fields;

            if (destinationField.Figure == null || destinationField.Figure.Player != game.PlayerOnTurn)
            {
                // That is to write less code by using the same + and - for both players

                int deltaCols = destinationField.Id % 10 - fieldFigure.Id % 10;
                int deltaRows = destinationField.Id / 10 - fieldFigure.Id / 10;

                // Check if the figure can jump so

                if (fieldFigure.Figure != null)
                {
                    switch (fieldFigure.Figure)
                    {
                        case King king:
                            result = deltaCols < 2 && deltaRows < 2 && deltaCols > -2 && deltaRows > -2 && (deltaCols != 0 || deltaRows != 0);
                            break;
                        case Queen queen:
                            result = ValidateFieldsTillAimAreFree(game, boardService, fieldFigure, destinationField);
                            break;
                        case Rook rook:
                            result = ValidateFieldsTillAimAreFree(game, boardService, fieldFigure, destinationField);

                            // Edge case for casteling
                            // King is always on field 0,4 with id 40, bec. the board is reversed for player 2
                            King? k = game.Board.Fields[0, 4].Figure as King;

                            if (rook != null && rook.IsFirstMove && k != null && k.IsFirstMove)
                            {
                                // check if fields are free
                                int colDeltaToKing = k.Id % 10 - rook.Id % 10;

                                result = ValidateFieldsTillAimAreFree(game, boardService, fieldFigure, game.Board.Fields[0, 4]);
                            }
                            break;
                        case Bishop bishop:
                            result = ValidateFieldsTillAimAreFree(game, boardService, fieldFigure, destinationField);
                            break;
                        case Knight knight:
                            result = ((deltaCols == 2 || deltaCols == -2) && (deltaRows == 1 || deltaRows == -1)) || ((deltaCols == 1 || deltaCols == -1) && (deltaRows == 2 || deltaRows == -2));
                            break;
                        case Pawn pawn:
                            result = ValidateEdgeCaseForPawn(game, boardService, fieldFigure, destinationField);
                            break;
                    }
                }
            }
            else
            {
                throw new Exception("Field is blocked by own figure");
            }

            // Step one check if field is blocked by own figure

            return result;
        }
        public bool ValidateFieldsTillAimAreFree(Game game, IBoardService boardService, Field f, Field destination)
        {
            // Check if figure can run this way must be done before calling this method
            bool result = true;
            Field[,] fields = game.Board.Fields;
            Field startField = f;
            Field destinationField = destination;


            int colDelta = destinationField.Id % 10 - startField.Id % 10;
            int rowDelta = destinationField.Id / 10 - startField.Id / 10;



            if (startField.Figure != null)
            {
                switch (startField.Figure.GetType())
                {
                    case Type t when t == typeof(Queen):
                        if (colDelta == 0) // Case 1 straigt movement
                        {
                            if (rowDelta > 0)
                            {
                                for (int i = 1; i < rowDelta && !result; i++) // Starting with one not to trigger own field
                                {
                                    if (fields[startField.Id % 10, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = -1; i > rowDelta && !result; i--)
                                {
                                    if (fields[startField.Id % 10, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }
                        else // Case 2 diagonal movement
                        {
                            if (colDelta > 0)
                            {
                                for (int i = 1; i < rowDelta && !result; i++)
                                {
                                    for (int x = 1; x < colDelta && !result; x++)
                                    {
                                        if (game.Board.Fields[startField.Id % 10 + x, startField.Id / 10 + i].Figure != null)
                                        {
                                            result = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = -1; i > rowDelta && !result; i--)
                                {
                                    for (int x = -1; x > colDelta && !result; x--)
                                    {
                                        if (fields[startField.Id % 10 + x, startField.Id / 10 + i].Figure != null)
                                        {
                                            result = false;
                                        }
                                    }
                                }
                            }

                        }
                        break;
                    case Type t when t == typeof(Rook):
                        if (colDelta == 0)
                        {
                            if (rowDelta > 0)
                            {
                                for (int i = 1; i < rowDelta && result; i++)
                                {
                                    if (fields[startField.Id % 10, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 1; i > rowDelta && result; i--)
                                {
                                    if (fields[startField.Id % 10, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (colDelta > 0)
                            {
                                for (int i = 1; i < colDelta && result; i++)
                                {
                                    if (fields[startField.Id % 10 + i, startField.Id / 10].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = -1; i > colDelta && result; i--)
                                {
                                    if (fields[startField.Id % 10 + i, startField.Id / 10].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }

                        break;
                    case Type t when t == typeof(Bishop):
                        if (colDelta > 0)
                        {
                            for (int i = 1; i < rowDelta && result; i++)
                            {
                                for (int x = 1; x < colDelta && result; x++)
                                {
                                    if (fields[startField.Id % 10 + x, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; i > rowDelta && result; i--)
                            {
                                for (int x = -1; x > colDelta && result; x--)
                                {
                                    if (fields[startField.Id % 10 + x, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }
                        break;
                    case Type t when t == typeof(Pawn): // Only when moving two fields forward in edge case first move
                        if (((Pawn)f.Figure).IsFirstMove && Math.Abs(rowDelta) == 2 && colDelta == 0)
                        {
                            for (int i = 1; i < rowDelta + 1 && !result; i++)
                            {
                                if (game.Board.Fields[startField.Id % 10, startField.Id / 10 + i].Figure != null)
                                {
                                    result = false;
                                }
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                }
            }
            else
            {
                throw new ArgumentNullException("No figure");
            }




            return result;
        }
        public bool ValidateEdgeCaseForPawn(Game game, IBoardService boardService, Field pawnField, Field destinationField)
        {
            bool result = false;
            Pawn? pawn = pawnField.Figure as Pawn;
            Field[,] fields = game.Board.Fields;

            int colDelta = destinationField.Id % 10 - pawnField.Id % 10;
            int rowDelta = Math.Abs(destinationField.Id / 10 - pawnField.Id / 10);



            // Case 1. default straigt movement

            if (pawn != null)
            {
                if (colDelta == 0 && rowDelta == 1 && destinationField.Figure == null)
                {
                    result = true;
                }
                else if (pawn.IsFirstMove && ValidateFieldsTillAimAreFree(game, boardService, pawnField, destinationField)) // Case 2. edgecase first move two fields
                {
                    result = true;
                }
                else if (colDelta == 1 && rowDelta == 1 && destinationField.Figure != null && destinationField.Figure.Player != game.PlayerOnTurn) // Case 3. edgecase diagonal throw
                {
                    result = true;
                }
                else if (colDelta == -1 && rowDelta == 1 && destinationField.Figure == null) // Case 4. en passant
                {
                    Field behindAim = fields[destinationField.Id % 10, destinationField.Id / 10 - 1];

                    if (behindAim.Figure != null && behindAim.Figure.GetType() == typeof(Pawn) && ((Pawn)behindAim.Figure).IsSecondMove)
                    {
                        result = true;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Field is not a pawn");
            }

            return result;
        }
        public bool ChangeFigure(Pawn pawn, Figure figure)
        {
            throw new NotImplementedException();
        }
    }
}
