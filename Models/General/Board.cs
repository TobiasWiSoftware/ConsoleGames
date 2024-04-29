using Chess.Models.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.General
{
    public class Board
    {
        // Singleton only one board

        private static Board? _board;

        // Readonly properties bec. they will not change under game
        public Field[,] Fields { get; set; } = new Field[8, 8];
        private Board(Player player1, Player player2)
        {

            Player settedPlayer = player1;
            int figureIndex = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Fields between index 2 and 5 are empty
                    if (row > 1 && row < 6)
                    {
                        Fields[col, row] = new Field(row * 10 + col);
                    }
                    else
                    {
                        // Field with figures rows 0, 1, 6, 7

                        if (row < 2)
                        {
                            // Player 1
                            settedPlayer = player1;
                        }
                        else
                        {
                            settedPlayer = player2;
                        }


                        if (row == 0 || row == 7)
                        {
                            Figure? figure = null;
                            // Advanced figures
                            switch (col)
                            {
                                case 0:
                                    figure = new Rook(figureIndex++, settedPlayer, false);
                                    break;
                                case 1:
                                    figure = new Knight(figureIndex++, settedPlayer);
                                    break;
                                case 2:
                                    figure = new Bishop(figureIndex++, settedPlayer);
                                    break;
                                case 3:
                                    figure = new Queen(figureIndex++, settedPlayer);
                                    break;
                                case 4:
                                    figure = new King(figureIndex++, settedPlayer);
                                    break;
                                case 5:
                                    figure = new Bishop(figureIndex++, settedPlayer);
                                    break;
                                case 6:
                                    figure = new Knight(figureIndex++, settedPlayer);
                                    break;
                                case 7:
                                    figure = new Rook(figureIndex++, settedPlayer, true);
                                    break;

                            }
                            if (figure != null)
                                Fields[col, row  ] = new(row * 10 + col, figure);
                        }
                        else
                        {
                            // Pawns

                            Fields[col, row] = new(row * 10 + col, new Pawn(figureIndex++, settedPlayer));
                        }
                    }
                }
            }

        }
        public static Board getTheBoard(Player player1, Player player2) => _board ??= new Board(player1, player2);
    }
}
