using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.General
{
    public class Move
    {
        private static Move? _lastMove = null;
        public Figure Figure { get => _figure; }
        public Field DestinationPosition { get => _destinationPosition; }
        public static bool ActivateEnPassante { get; set; }

        private Field _destinationPosition;

        private Figure _figure;

        // Validation before creating the obj
        private Move(Figure f, Field field)
        {
            _figure = f;
            _destinationPosition = field;
            ActivateEnPassante = false;
        }

        ~Move()
        {

        }
        // Singelton to make sure it changes every move to the last one
        public static Move? GetLastMove()
        {
            return _lastMove;
        }

        public static Move SetLastMove(Figure f, Field destinationField)
        {
            return _lastMove = new(f, destinationField);
        }

        public static void SetLastMove()
        {
            _lastMove = null;
        }

    }
}
