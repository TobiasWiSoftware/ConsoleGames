using Chess.Models.Figures;
using Chess.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chess.Service
{
    public class GameService : IGameService
    {
        public Game? Game { get => _game; }
        public void SetUpGame(string p1name, string p2name)
        {

            if (p1name.Length < 10 && p1name.Trim().Length > 1 || p2name.Length < 10 && p2name.Trim().Length > 1)
            {
                if (_game == null)
                {
                    _game = Game.getTheGame(new Player(0, p1name), new Player(1, p2name));
                    _boardService.SetUpBoard(_game.Player1, _game.Player2);
                }
                else
                {
                    throw new Exception("Game already set");
                }
            }
            else
            {
                throw new Exception("Playernames < 10 char and min 1 char without whitespaces");
            }

        }

        private Game? _game;

        private readonly IBoardService _boardService;
        public GameService(IBoardService boardService)
        {
            _boardService = boardService;
        }
        public void ChangePlayerOnTurn()
        {
            if (_game != null)
            {
                if (_game.PlayerOnTurn == _game.Player1)
                {
                    _game.PlayerOnTurn = _game.Player2;
                }
                else
                {
                    _game.PlayerOnTurn = _game.Player1;

                }
            }
        }
        public Board GetCurrentBoard() => _boardService.Board != null ? _boardService.Board : throw new Exception("Error on board injection");
        public Field GetFieldOfFigure(Figure figure)
        {
            if (_game != null)
                return _boardService.GetFieldOfFigure(figure);
            else
            {
                throw new Exception("Game not set");
            }
        }
        public bool SelectField(Field f)
        {
            throw new NotImplementedException();
        }
        public bool MoveFigure(Figure figure, Field field)
        {
            if (_game != null)
            {
                bool result = _boardService.MoveFigure(this._game, figure, field);
                Player opponend = _game.PlayerOnTurn == _game.Player1 ? _game.Player2 : _game.Player1;
                opponend.IsCheck = _boardService.IsCheck(opponend);
                return result;
            }
            else
            {
                throw new Exception("Game not set");
            }
        }

        public bool MoveFigure(Rook rook)
        {
            if (_game != null)
            {
                bool result = _boardService.MoveFigure(this.Game, rook);
                Player opponend = _game.PlayerOnTurn == _game.Player1 ? _game.Player2 : _game.Player1;
                opponend.IsCheck = _boardService.IsCheck(opponend);
                return result;
            }
            else
            {
                throw new Exception("Game not set");
            }
        }
        public bool RunGame() => _game != null && _game.IsActive;

    }
}
