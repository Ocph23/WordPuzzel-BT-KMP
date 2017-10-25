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
    /// Interaction logic for AddNewSoal.xaml
    /// </summary>
    public partial class AddNewSoal : Window
    {
        public AddNewSoal()
        {
            InitializeComponent();
            this.Options = new ObservableCollection<Models.Option>();
            this.Options.Add(new Option());
            this.Options.Add(new Option());
            this.Options.Add(new Option());
            this.Options.Add(new Option());

            this.DataContext = this;
        }

        public ObservableCollection<Option> Options { get; set; }
        public Soal SoalCreated { get; private set; }

        private void simpan_Click(object sender, RoutedEventArgs e)
        {
            //validate
            if(ValidateValue())
            {

                using (var db = new OcphDbContext())
                {
                    var trans = db.Connection.BeginTransaction();
                    try
                    {
                        var soal = new Models.Soal { Level = Level.Easy, Value = soalTxt.Text };
                        soal.Id = db.Soals.InsertAndGetLastID(soal);
                        if(soal.Id>0)
                        {
                            foreach(var item in Options)
                            {
                                item.SoalId = soal.Id.Value;
                               item.Id= db.Options.InsertAndGetLastID(item);
                                if (item.Id <= 0)
                                    throw new SystemException("Data Gagal Dapat Disimpan");
                                else
                                {
                                    soal.AddOption(item);
                                }
                            }

                            trans.Commit();
                            this.SoalCreated = soal;
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {

                        trans.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
            }else
            {
                MessageBox.Show("Periksa Kembali Data Anda");
            }

        }

        private bool ValidateValue()
        {
            var valid = true;
            var trueAnswerCount = 0;
            foreach(var item in Options )
            {
                if (item.IsTrueAnswer)
                    trueAnswerCount++;
                if(string.IsNullOrEmpty(item.Value))
                {
                    valid = false;
                    break;
                }
            }
            if (valid && trueAnswerCount == 1 && !string.IsNullOrEmpty(soalTxt.Text))
            {
                return true;
            }
            else
                return false;

        }

        private void batal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
