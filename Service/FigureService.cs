﻿using Chess.Models.Figures;
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

                            if (!result)
                            {
                                Field? kingField = null;

                                if (game.PlayerOnTurn == game.Player1)
                                    kingField = game.Board.Fields[3, 0];
                                else
                                    kingField = game.Board.Fields[4, 0];

                                if (rook != null && rook.IsFirstMove && kingField != null && kingField.Figure != null && kingField.Figure is King && (kingField.Figure as King).IsFirstMove)
                                {

                                    result = ValidateFieldsTillAimAreFree(game, boardService, fieldFigure, game.Board.Fields[kingField.Id % 10 + (rook.IsKingRook && game.PlayerOnTurn == game.Player1 ? 1 : -1), kingField.Id / 10]);
                                }
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




            if (startField.Figure != null && destinationField.Figure == null || destinationField.Figure.Player != game.PlayerOnTurn)
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
                        else if (colDelta == rowDelta) // Case 2 diagonal movement
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
                        else
                        {
                            result = false;
                        }
                        break;
                    case Type t when t == typeof(Rook):
                        // Edge case for no straight movement

                        if (colDelta != 0 && rowDelta != 0)
                        {
                            result = false;
                        }

                        if (colDelta == 0 && result)
                        {
                            if (rowDelta != 0 && rowDelta != 1 && rowDelta != -1)
                            {
                                for (int i = 1; i < rowDelta && result; i++)
                                {
                                    if (fields[startField.Id % 10, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                                for (int i = rowDelta + 1; i < 0 && result; i++)
                                {
                                    if (fields[startField.Id % 10, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                            else if (rowDelta == 0)
                            {
                                result = false;
                            }

                        }
                        else
                        {
                            if (colDelta != 0 && colDelta != 1 && colDelta != -1)
                            {
                                for (int i = 1; i < colDelta - 1 && result; i++)
                                {
                                    if (fields[startField.Id % 10 + i, startField.Id / 10].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                                for (int i = colDelta + 1; i < 0 && result; i++)
                                {
                                    if (fields[startField.Id % 10 + i, startField.Id / 10].Figure != null)
                                    {
                                        result = false;
                                    }
                                }
                            }
                            else if (colDelta == 0)
                            {
                                result = false;
                            }
                        }

                        break;
                    case Type t when t == typeof(Bishop):
                        if (colDelta > 0 && rowDelta == colDelta)
                        {
                            int x = 0;
                            for (int i = 1; i < rowDelta && result; i++)
                            {
                                x++;
                                while (x < colDelta && result)
                                {
                                    if (fields[startField.Id % 10 + x, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        x++;
                                        break;
                                    }
                                }
                            }
                            x = -1;
                            for (int i = -1; i > rowDelta && result; i--)
                            {
                                x++;
                                while (x < colDelta && result)
                                {
                                    if (fields[startField.Id % 10 + x, startField.Id / 10 + i].Figure != null)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        x--;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case Type t when t == typeof(Pawn): // Only when moving two fields forward in edge case first move
                        if (((Pawn)f.Figure).TwoStepPossible && Math.Abs(rowDelta) == 2 && colDelta == 0)
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
            int rowDelta = destinationField.Id / 10 - pawnField.Id / 10;




            if (pawn != null && rowDelta > 0)
            {
                // Case 1. default straigt movement
                if ((colDelta == 0 && rowDelta == 1 || colDelta == 0 && rowDelta == -1) && destinationField.Figure == null)
                {
                    result = true;
                    pawn.TwoStepPossible = false;
                }
                // Case 2. edge case first move two fields
                else if (pawn.TwoStepPossible && ValidateFieldsTillAimAreFree(game, boardService, pawnField, destinationField))
                {
                    result = true;
                }
                // Case 3. edgecase diagonal throw
                else if ((colDelta == 1 && rowDelta == 1 && destinationField.Figure != null && destinationField.Figure.Player != game.PlayerOnTurn) || (colDelta == -1 && rowDelta == 1 && destinationField.Figure != null && destinationField.Figure.Player != game.PlayerOnTurn))
                {
                    result = true;
                    pawn.TwoStepPossible = false;
                }
                // Case 4. en passant
                else if ((colDelta == 1 && rowDelta == 1 && destinationField.Figure == null) || (colDelta == -1 && rowDelta == 1 && destinationField.Figure == null))
                {
                    Field behindAim = fields[destinationField.Id % 10, destinationField.Id / 10 - 1];

                    if (behindAim.Figure != null && behindAim.Figure.GetType() == typeof(Pawn) && ((Pawn)behindAim.Figure).EnPassantePossible)
                    {
                        result = true;
                    }
                }
            }
            else
            {
                throw new Exception("Only forward movement allowed");
            }

            return result;
        }
        public bool ChangeFigure(Pawn pawn, Figure figure)
        {
            throw new NotImplementedException();
        }
    }
}
