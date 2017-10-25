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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameWordPuzzel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGame_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.DialogBox(this);
            form.Show();
            this.Hide();
        }

        private void btnadmin_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.LoginView(this);
            form.ShowDialog();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void kuis_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.QuizView(this);
            form.Show();
            this.Hide();
        }
    }
}
