using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameWordPuzzel
{
    public static class Helper
    {
        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            return list.ElementAt(new Random(DateTime.Now.Millisecond).Next(list.Count()));
        }


        public static Diagonal FindOnesFromLeftToRight(int[,] matrix,int row, int col)
        {
            int rows = matrix.GetUpperBound(0) + 1;
            int columns = matrix.GetUpperBound(1) + 1;
            int lastColumnIndex = columns - 1;
            int lastRowIndex = rows - 1;

            int max = 0;
            int i = row;
            int j = col;
            int lastrow = 0;
            int lastcol = 0;
            //calculate 1's from left to right
            var da = new Diagonal()
            {
                RowStart = row,
                CollStart = col,
                Direction = Direction.LeftToRight,
                Number = max
            };

            while ((i > 0 && i < 9) && (j > 0 && j < 9))
            {
                i = i - 1;
                da.RowStart = i;
                j = j - 1;
                da.CollStart = j;
                da.FirstCount++;
            }

            while (j <= columns)
            {

                if (j <= lastColumnIndex && i <= lastRowIndex)
                {
                    lastrow = i;
                    lastcol = j;
                    max++;
                }
                j++;
                i++;
            }
            if (max > 0)
            {
                da.CollEnd = lastcol;
                da.RowEnd = lastrow;
                da.Number = max;
                return da;

            }
            else
                return null;
        }

        public static Diagonal FindOnesFromRightToLeft(int[,] matrix,  int row, int col)
        {
            int rows = matrix.GetUpperBound(0) + 1;
            int columns = matrix.GetUpperBound(1) + 1;
            int lastColumnIndex = columns - 1;
            int lastRowIndex = rows - 1;

            int max = 0;
            int i = row;
            int j = col;
            int lastrow = 0;
            int lastcol = 0;
            //calculate 1's from left to right
            var da = new Diagonal()
            {
                RowStart = row,
                RowEnd = lastrow,
                CollStart = col,
                CollEnd = lastcol,
                ColumnIndex = j,
                Direction = Direction.RightToLeft,
                Number = max
            };

            while ((i > 0 && i < 9) && (j > 0 && j < 9))
            {
                i = i - 1;
                da.RowStart = i;
                j = j + 1;
                da.CollStart = j;
                da.FirstCount++;
            }

            while (j >= 0)
            {
                if ((i >= 0 && i <= 9) && (j >= 0 && j <= 9))
                {
                    lastrow = i;
                    lastcol = j;
                    max++;
                }
                j--;
                i++;
            }
            if (max > 0)
            {
                da.CollEnd = lastcol;
                da.RowEnd = lastrow;
                da.Number = max;
                return da;
            }else
            {
                return null;
            }

        }

        public static SolidColorBrush PickBrush()
        {
            Random r = new Random();
            byte red =(byte) r.Next(0, byte.MaxValue + 1);
            byte green = (byte)r.Next(0, byte.MaxValue + 1);
            byte blue = (byte)r.Next(0, byte.MaxValue + 1);
            var brush = new SolidColorBrush(Color.FromArgb(225,red, green, blue));
            brush.Freeze();
            return brush;
        }

        public static SolidColorBrush BindingBrush(SolidColorBrush current,SolidColorBrush newcolor)
        {
            Color foreground = current.Color;
            Color background = newcolor.Color;

            byte opacity = 75;

            byte r =(byte)( (opacity * (foreground.R - background.R) / 100) + background.R);
            byte g = (byte)((opacity * (foreground.G - background.G) / 100) + background.G);
            byte b = (byte)((opacity * (foreground.B - background.B) / 100) + background.B);

            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(opacity, r, g, b));
            brush.Freeze();
            return brush;
        }

        internal static Border NewBorder(Brush color)
        {
            Border border = new Border();
            border.BorderBrush = color;
            border.Opacity = 0.5;
            border.BorderThickness = new Thickness(10, 10, 10, 10);
            Grid.SetRow(border, 1);
            Grid.SetColumn(border, 1);
            return border;
        }

        internal static string RandomString(Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        
        internal static int RandomPosisionColumn(Random rcolumn)
        {
            const string chars = "1234567890";
            return Convert.ToInt32(new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[rcolumn.Next(s.Length)]).ToArray()));
        }

       
        public static int RandomPosisionRow(Random rrow)
        {
            const string chars = "1234567890";
            return Convert.ToInt32(new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[rrow.Next(s.Length)]).ToArray()));
        }

        public static Arah RandomArah(Random rarah)
        {
            const string chars = "123";
            var res = Convert.ToInt32(new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[rarah.Next(s.Length)]).ToArray()));
            if (res == 1)
            {
                return Arah.Horizontal;
            }
            else if (res == 2)
                return Arah.Vertical;
            else
                return Arah.Diagonal;
        }

    }
}
