using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.General
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get => _name; }
        public bool IsCheck { get; set; }
        public bool IsCheckmate { get; set; }

        private string _name;
        public List<Figure> Figures { get; set; } = new();
        public Move? LastMove { get; set; }
        public Player(int id, string name)
        {
            Id = id;
            _name = name;
            IsCheck = false;
            IsCheckmate = false;
        }
        public override bool Equals(object? obj)
        {
            Player? other = obj as Player;

            return other != null && other.Id == Id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
