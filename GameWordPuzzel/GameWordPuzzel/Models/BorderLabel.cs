using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameWordPuzzel.Models
{
   public class BorderLabel :Border
    {
        public BorderLabel(string name)
        {
            this.Name = name;
            Margin = new Thickness(5);
            Background = Brushes.Beige;
            CornerRadius = new CornerRadius(20);
            BorderThickness = new Thickness(5);
            BorderBrush = Brushes.Beige;
            Padding = new Thickness(5);

            this.Text = new TextBlock { FontSize =16, Text = name};
            this.Child = this.Text;
        }

        public void Found()
        {
            this.Text.TextDecorations = TextDecorations.Strikethrough;
            this.Text.Foreground = Brushes.Red;
            this.IsFound = true;
        }

        public TextBlock Text { get; set; }
        public bool IsFound { get; private set; }
    }
}
