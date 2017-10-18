using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{
    public class Cordinate
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public Cordinate(int column, int row)
        {
            this.Column = column;
            this.Row = row;
        }
    }
}
