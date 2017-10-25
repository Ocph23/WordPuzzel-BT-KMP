using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GameWordPuzzel.Models;

namespace GameWordPuzzel.Views
{
    /// <summary>
    /// Interaction logic for QuizView.xaml
    /// </summary>
    public partial class QuizView : Window
    {
        private MainWindow mainWindow;

        public QuizView(MainWindow mainWindow)
        {
            InitializeComponent();
            this.Loaded += QuizView_Loaded;
            this.mainWindow = mainWindow;
        }

        public ObservableCollection<Soal> Soals { get; private set; }
        public CollectionView SoalView { get; private set; }

        private void QuizView_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new OcphDbContext())
            {
                var soals = db.Soals.Select();
                var listSoal = Helper.ShuffleList<Soal>(soals.ToList());
                var nomor = 0;
                foreach (var item in listSoal)
                {
                    nomor++;
                    item.Number = nomor;
                    var res = db.Options.Where(O => O.SoalId == item.Id).ToList();
                    var list = Helper.ShuffleList<Option>(res);
                    foreach (var option in list)
                    {
                        item.AddOption(option);
                    }
                }

               
                this.Soals = new ObservableCollection<Models.Soal>(listSoal);
                this.SoalView = (CollectionView)CollectionViewSource.GetDefaultView(Soals);
                this.SoalView.Refresh();
            }
            this.DataContext = this;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            this.Close();
        }

        private void finish_Click(object sender, RoutedEventArgs e)
        {
            var answerCount = 0;
            var trueAnswerCount = 0;
            foreach(var item in Soals)
            {
                if(item.CorrectAnswer!=null)
                {
                    answerCount++;
                    if (item.CorrectAnswer.Value)
                    {
                        trueAnswerCount++;
                    }
                }
            }

            if(answerCount<Soals.Count)
            {
                MessageBox.Show("Silahkan Jawab Semua Pertanyaan ","Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }else
            {
                MessageBox.Show(string.Format("Jawaban Benar : {0}",trueAnswerCount), "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    }
}
