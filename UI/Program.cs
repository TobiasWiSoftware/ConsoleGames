using Chess.Models.Figures;
using Chess.Models.General;
using Chess.Service;
using Chess.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Xml.Linq;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IBoardService, BoardService>();
        services.AddTransient<IFigureService, FigureService>();
        services.AddTransient<IBoardService>(provider => new BoardService(provider.GetRequiredService<IFigureService>()));
        services.AddTransient<IGameService>(provider =>
           new GameService(provider.GetRequiredService<IBoardService>()));
    })
    .Build();

// Resolve GameService and run it
var gameService = host.Services.GetRequiredService<IGameService>();



Console.OutputEncoding = System.Text.Encoding.UTF8;
await Window.MaximizeConsoleWindow();

// Max ui size 20 chars
int maxWidthOfConsole = Console.WindowWidth;
int maxHeightOfConsole = Console.WindowHeight;
int cursorTop = maxHeightOfConsole / 5;
int cursorLeft = (maxWidthOfConsole - 20) / 2;

Console.Write($"Player1: ");
string? p1Name = "Tob"; // Console.ReadLine();
Console.Clear();
Console.Write("Player2: ");
string? p2Name = "Seb"; // Console.ReadLine();
Console.Clear();
Console.ForegroundColor = ConsoleColor.White;


gameService.SetUpGame(p1Name, p2Name);


// Setup the host


if (p1Name != null && p2Name != null)
{


    while (gameService.RunGame())
    {
        cursorLeft = (maxWidthOfConsole - 20) / 2;
        cursorTop = maxHeightOfConsole / 5;
        Console.SetCursorPosition(cursorLeft, cursorTop++);
        Console.Write("C H E S S  I N  C #");
        cursorTop++;


        Board board = gameService.GetCurrentBoard();

        // Only for drawing the board, no logic
        for (int row = -1; row < board.Fields.GetLength(1); row++)
        {
            for (int col = -1; col < board.Fields.GetLength(0); col++)
            {
                if (col > -1 && row > -1)
                {

                    Figure? figure = board.Fields[col, row].Figure;
                    if (figure != null)
                    {


                        string symbol = string.Empty;
                        Type type = figure.GetType();

                        switch (type)
                        {
                            case Type t when t == typeof(Rook):
                                symbol = "♜";
                                break;
                            case Type t when t == typeof(Knight):
                                symbol = "♞";
                                break;
                            case Type t when t == typeof(Bishop):
                                symbol = "♝";
                                break;
                            case Type t when t == typeof(Pawn):
                                // make a pawn in default color
                                symbol = "♙";
                                break;
                            case Type t when t == typeof(King):
                                symbol = "♔";
                                break;
                            case Type t when t == typeof(Queen):
                                symbol = "♕";
                                break;
                        }

                        if (figure.Player.Id == gameService.Game.Player1.Id)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;

                        }
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        Console.Write(symbol);
                        cursorLeft += 2;
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                    else
                    {
                        Console.SetCursorPosition(cursorLeft, cursorTop);
                        Console.Write('☐');
                        cursorLeft += 2;
                    }

                }
                else if (col == -1)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    Console.Write(row + 1);
                    cursorLeft += 2;

                }
                else // Row for A - G text
                {
                    Console.SetCursorPosition(cursorLeft++, cursorTop);
                    Console.Write(Convert.ToChar(col + 65));
                    if (row == -1)
                    {
                        cursorLeft++;
                    }
                }
                cursorLeft++;
            }
            cursorTop++;
            cursorLeft = (maxWidthOfConsole - 20) / 2;
            if (row == -1)
            {
                cursorTop++;
            }
        }

        cursorTop += 2;
        bool correctInformation = false;
        Field? fieldOfFigure = null;

        // Choose figure
        do
        {
            Console.SetCursorPosition(cursorLeft, cursorTop++);
            Console.Write((gameService != null && gameService.Game! != null ? gameService.Game.PlayerOnTurn.Name : throw new Exception("No Player on turn")) + " choose your figure: ");
            try
            {
                string? s = Console.ReadLine();
                if (s != null)
                {
                    s = s.ToUpper();
                    int c = s[0] - 65;
                    int r = ((int)Char.GetNumericValue(s[1]) - 1);
                    fieldOfFigure = gameService.GetCurrentBoard().Fields[c, r];

                    if (fieldOfFigure.Figure == null || fieldOfFigure.Figure.Player != gameService.Game.PlayerOnTurn)
                    {
                        Console.SetCursorPosition(cursorLeft, cursorTop++);
                        throw new Exception("There is none of your figures on the field");
                    }
                    correctInformation = true;

                    Console.SetCursorPosition(cursorLeft, cursorTop++);
                    Console.Write("You choose " + fieldOfFigure + " with " + fieldOfFigure.Figure + " - " + " exit with EX");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } while (!correctInformation);

        correctInformation = false;

        // Choose destination field
        do
        {
            Console.SetCursorPosition(cursorLeft, cursorTop++);
            Console.Write("Enter your target field: ");

            try
            {
                string? s = Console.ReadLine();
                if (s != null)
                {
                    s = s.ToUpper().Trim();
                    if (s != "EX" && fieldOfFigure != null && fieldOfFigure.Figure != null)
                    {
                        s = s.ToUpper();
                        Field destination =  gameService.Game.Board.Fields[s[0] - 65, ((int)char.GetNumericValue(s[1]) - 1)];
                        bool movePossible = gameService.MoveFigure(fieldOfFigure.Figure, destination);
                        if(movePossible)
                        {
                            correctInformation = true;
                        }
                        else
                        {
                            throw new Exception("Move not possible");
                        }
                    }
                    else
                    {
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop++);
                Console.WriteLine(ex.Message);
            }
        } while (!correctInformation);

        if (correctInformation)
        {
            gameService.ChangePlayerOnTurn();
        }
        // When Pawn gets to end of field
        correctInformation = false;


        Console.Clear();
    }
}





