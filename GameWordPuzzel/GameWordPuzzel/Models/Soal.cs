using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GameWordPuzzel.Models
{

    public delegate void delChangeAnswer(bool answer,Option option);
    [TableName("Soal")]
   public class Soal:DAL.BaseNotifyProperty
    {
        private int? id;
        [PrimaryKey("Id")]
        [DbColumn("Id")]
        public int? Id
        {
            get { return id; }
            set { id = value;
                OnPropertyChanged();
            }
        }

        private string _value;
        [DbColumn("Value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }

        private Level level;
        [DbColumn("Level")]
        public Level Level
        {
            get { return level; }
            set { level = value; OnPropertyChanged(); }
        }

        public bool? CorrectAnswer { get; private set; }

        public void AddOption(Option option)
        {
            option.delAnswer = new delChangeAnswer(SetAnswer);
            this.list.Add(option);
            Options.Refresh();
        }

        private void SetAnswer(bool answer,Option option)
        {
            this.CorrectAnswer = answer;
           
        }

        private List<Option> list=new List<Option>();
        public CollectionView Options { get; set; }
        public int Number { get; internal set; }

        public Soal()
        {
            //Random rnd = new Random();
            Options = (CollectionView)CollectionViewSource.GetDefaultView(list);
        }

    }
}
