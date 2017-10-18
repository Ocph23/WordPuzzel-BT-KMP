using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel
{
   public class Diagonal
    {
        public int RowIndex;
        public int ColumnIndex;
        public Direction Direction;
        public int Number;
        public int FirstCount { get; set; }
        public int SecoundCount
        {
            get
            {
                return Number - FirstCount;
            }
        }
        public int RowStart { get; internal set; }
        public int RowEnd { get; internal set; }
        public int CollStart { get; internal set; }
        public int CollEnd { get; internal set; }
    }
    public enum Direction
    {
        LeftToRight,
        RightToLeft
    }
}
