using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel
{
    class EnumCollection
    {
    }

    public enum Arah
    {
        Horizontal, Vertical, Diagonal
    }

    public enum ArahPanah
    {
        HorizontalToRight, HorizontalToLeft, VerticalToTop,VerticalToBottom, DiagonalToDownRight, DiagonalToDownLeft, DiagonalToUpRight, DiagonalToUpLeft,
        None
    }

    public enum Level
    {
        Easy,Middle,Advance
    }
}
