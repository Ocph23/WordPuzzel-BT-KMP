using GameWordPuzzel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for DataKataView.xaml
    /// </summary>
    public partial class DataKataView : Window,INotifyPropertyChanged
    {
        private Kategori _selectedCategories;

        public DataKataView()
        {
            InitializeComponent();

            using (var db = new OcphDbContext())
            {
                var katas = db.Categories.Insert(new Kategori { Id = 0, Name = "Music" });
            }


            var list = new List<Kategori>();
            var kategori1 = new Kategori { Id = 1, Name = "Music" };
            kategori1.DaftarKata = new List<Kata>();
            kategori1.DaftarKata.Add(new Kata { Id = 1, KategoriId = kategori1.Id, Value = "POP" });
            kategori1.DaftarKata.Add(new Kata { Id = 2, KategoriId = kategori1.Id, Value = "DANGDUT" });
            kategori1.DaftarKata.Add(new Kata { Id = 3, KategoriId = kategori1.Id, Value = "ROCK" });
            kategori1.DaftarKata.Add(new Kata { Id = 4, KategoriId = kategori1.Id, Value = "KERONCONG" });
            list.Add(kategori1);

            var kategori2 = new Kategori { Id = 2, Name = "EKONOMI" };
            kategori2.DaftarKata = new List<Kata>();
            kategori2.DaftarKata.Add(new Kata { Id = 1, KategoriId = kategori2.Id, Value = "MONETER" });
            kategori2.DaftarKata.Add(new Kata { Id = 2, KategoriId = kategori2.Id, Value = "KREDIT" });
            kategori2.DaftarKata.Add(new Kata { Id = 3, KategoriId = kategori2.Id, Value = "DEBET" });
            kategori2.DaftarKata.Add(new Kata { Id = 4, KategoriId = kategori2.Id, Value = "AKUNTANSI" });
            list.Add(kategori2);

            this.Kategories = new ObservableCollection<Kategori>(list);
            this.Daftar = new ObservableCollection<Kata>();
            this.KategoriesView = (CollectionView)CollectionViewSource.GetDefaultView(Kategories);
            this.DaftarView = (CollectionView)CollectionViewSource.GetDefaultView(Daftar);
            this.DataContext = this;

          


        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ObservableCollection<Kategori> Kategories { get; private set; }
        public ObservableCollection<Kata> Daftar { get; private set; }
        public CollectionView KategoriesView { get; set; }
        public CollectionView DaftarView { get; set; }
        public Kategori CategorySelected {
            get { return _selectedCategories; }
            set
            {
                if(_selectedCategories!=value)
                {
                    _selectedCategories = value;
                    Daftar.Clear();
                    foreach(var item in value.DaftarKata)
                    {
                        Daftar.Add(item);
                    }
                    DaftarView.Refresh();
                    NotifyPropertyChanged();
                }
            }
        }

        private void DataKataView_Loaded(object sender, RoutedEventArgs e)
        {
           


        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if(textBox.Text!=string.Empty && CategorySelected!=null)
            {
                Daftar.Add(new Kata { Id = 8, KategoriId = CategorySelected.Id, Value = textBox.Text.ToUpper() });
                textBox.Text = string.Empty;
                DaftarView.Refresh();
            }
         
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            var id =Convert.ToInt32( ((Button)sender).CommandParameter);
            if (id > 0)
            {
                var item = Daftar.Where(O => O.Id == id).FirstOrDefault();
                if (item != null)
                {
                    Daftar.Remove(item);
                    DaftarView.Refresh();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
