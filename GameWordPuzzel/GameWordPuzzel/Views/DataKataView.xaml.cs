﻿using GameWordPuzzel.Models;
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
                var cats = db.Categories.Select().ToList();
                foreach(var item in cats)
                {
                    item.Words = db.Words.Where(O => O.KategoriId == item.Id).ToList();
                }
                this.Kategories = new ObservableCollection<Kategori>(cats);
                
                this.KategoriesView = (CollectionView)CollectionViewSource.GetDefaultView(Kategories);
                this.Daftar = new ObservableCollection<Kata>();
                this.DaftarView = (CollectionView)CollectionViewSource.GetDefaultView(Daftar);
                this.DataContext = this;

            }
            /*

            var list = new List<Kategori>();
            var kategori1 = new Kategori { Id = 1, Name = "Music" };
            kategori1.Words = new List<Kata>();
            kategori1.Words.Add(new Kata { Id = 1, KategoriId = kategori1.Id, Nilai = "POP" });
            kategori1.Words.Add(new Kata { Id = 2, KategoriId = kategori1.Id, Nilai = "DANGDUT" });
            kategori1.Words.Add(new Kata { Id = 3, KategoriId = kategori1.Id, Nilai = "ROCK" });
            kategori1.Words.Add(new Kata { Id = 4, KategoriId = kategori1.Id, Nilai = "KERONCONG" });
            list.Add(kategori1);

            var kategori2 = new Kategori { Id = 2, Name = "EKONOMI" };
            kategori2.Words = new List<Kata>();
            kategori2.Words.Add(new Kata { Id = 1, KategoriId = kategori2.Id, Nilai = "MONETER" });
            kategori2.Words.Add(new Kata { Id = 2, KategoriId = kategori2.Id, Nilai = "KREDIT" });
            kategori2.Words.Add(new Kata { Id = 3, KategoriId = kategori2.Id, Nilai = "DEBET" });
            kategori2.Words.Add(new Kata { Id = 4, KategoriId = kategori2.Id, Nilai = "AKUNTANSI" });
            list.Add(kategori2);
            */
            

          


        }


      
        public ObservableCollection<Kategori> Kategories { get; private set; }
        public ObservableCollection<Kata> Daftar { get; private set; }
        public CollectionView KategoriesView { get; set; }
        public CollectionView DaftarView { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Kategori CategorySelected {
            get { return _selectedCategories; }
            set
            {
                if(_selectedCategories!=value)
                {
                    _selectedCategories = value;
                    Daftar.Clear();
                    if (value.Words == null)
                        value.Words = new List<Kata>();

                    foreach(var item in value.Words)
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
                var text = textBox.Text.Trim().ToUpper();
                using (var db = new OcphDbContext())
                {
                    var newKata = new Kata { KategoriId = CategorySelected.Id.Value, Nilai = text };
                    newKata.Id = db.Words.InsertAndGetLastID(newKata);
                    if(newKata.Id>0)
                    {
                        Daftar.Add(newKata);
                    }
                }
              
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

        private void addkategori_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text != string.Empty)
            {
                var text = textBox.Text.Trim().ToUpper();
                using (var db = new OcphDbContext())
                {
                    var newCat = new Kategori { Name = text };
                    newCat.Id = db.Categories.InsertAndGetLastID(newCat);
                    if (newCat.Id > 0)
                    {
                        Kategories.Add(newCat);
                    }
                }
                textBox.Text = string.Empty;
                KategoriesView.Refresh();
            }

        }
    }
}
