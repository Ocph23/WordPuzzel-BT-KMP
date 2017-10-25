using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameWordPuzzel.Views
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
        private MainWindow mainWindow;

        public AdminPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void btnGame_Click(object sender, RoutedEventArgs e)
        {
            var form = new DataKataView();
            form.ShowDialog();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            this.Close();
        }

        private void soal_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.AdminAddSoal();
            form.ShowDialog();
        }
    }
}
