using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GameWordPuzzel.Models;

namespace GameWordPuzzel.Views
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : Window
    {
        private MainWindow mainWindow;
        public Models.Cordinate Selected { get; set; }
        public ObservableCollection<Cordinate> Cordinates { get; }

        public DialogBox(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.DataContext = this;
            var list = new List<Models.Cordinate>
            {
                new Cordinate(5, 5),
                new Cordinate(6, 6),
                new Cordinate(7, 7),
                new Cordinate(8, 8),
                new Cordinate(9, 9),
                new Cordinate(10, 10)
            };
            this.Cordinates = new ObservableCollection<Models.Cordinate>(list);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            this.Close();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.GameBoard(mainWindow,Selected);
            form.Show();
            this.Close();
        }
    }
}
