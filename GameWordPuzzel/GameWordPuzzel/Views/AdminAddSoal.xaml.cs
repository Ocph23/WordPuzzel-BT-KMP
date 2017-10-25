using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Runtime.CompilerServices;

namespace GameWordPuzzel.Views
{
    /// <summary>
    /// Interaction logic for AdminAddSoal.xaml
    /// </summary>
    public partial class AdminAddSoal : Window,INotifyPropertyChanged
    {
        private Soal _selected;

        public ObservableCollection<Models.Soal> Soals { get; set; }
        public CollectionView SoalView { get; set; }
        public Models.Soal Selected {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        public AdminAddSoal()
        {
            InitializeComponent();
            this.Loaded += AdminAddSoal_Loaded;
           


        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AdminAddSoal_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new OcphDbContext())
            {
                var soals = db.Soals.Select();
                foreach(var item in soals)
                {
                    var options = db.Options.Where(O => O.SoalId == item.Id).ToList();
                    foreach(var option in options)
                    {
                        item.AddOption(option);
                    }
                }
                this.Soals = new ObservableCollection<Models.Soal>(soals);
                this.SoalView = (CollectionView)CollectionViewSource.GetDefaultView(Soals);
                this.SoalView.Refresh();
            }
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var form = new Views.AddNewSoal();
            form.Show();
            if (form.SoalCreated!=null)
            {
                this.Soals.Add(form.SoalCreated);
                this.SoalView.Refresh();

            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            Soals.Remove(Selected);
        }

        private void batal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
