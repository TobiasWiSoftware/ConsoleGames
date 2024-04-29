using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.General
{
    public class Field
    {
        public int Id { get; set; }
        public Figure? Figure { get; set; }
        public Field(int col_row)
        {
            Id = col_row;
        }
        public Field(int col_row, Figure figure)
        {
            Id = col_row;
            Figure = figure;
        }
        public override string ToString()
        {
            return $"Feld {(char)(Id % 10 + 65)}{Id / 10 + 1}";
        }
    }
}
