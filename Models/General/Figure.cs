using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.General
{

    public class Figure
    {
        public int Id { get => _id; }
        public Player Player { get => _player; }

        private int _id;

        private Player _player;
        public Figure(int id, Player player)
        {
            _id = id;
            _player = player;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}";
        }
    }
}
