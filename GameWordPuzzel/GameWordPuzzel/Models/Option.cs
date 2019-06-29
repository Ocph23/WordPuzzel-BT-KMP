using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{
    [TableName("Pilihan")]
  public  class Option:BaseNotify
    {
              
        private int? _id;
        private string _value;
        private int _soalId;

        [PrimaryKey("Id")]
        [DbColumn("Id")]

        public int? Id
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value);
            }
        }
       
        [DbColumn("SoalId")]
        public int SoalId
        {
            get { return _soalId; }
            set
            {
                SetProperty(ref _soalId, value);
            }
        }
        
        [DbColumn("Value")]
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }


        private bool _isTrue;
        private bool _userSelected;
        internal delChangeAnswer delAnswer;

        [DbColumn("IsTrue")]
        public bool IsTrueAnswer
        {
            get { return _isTrue; }
            set { SetProperty(ref _isTrue, value); }
        }


        public bool UserSelected
        {
            get { return _userSelected; }
            set
            {
                
                if (value && IsTrueAnswer)
                {
                    delAnswer(true,this);
                }
                else if(value)
                    delAnswer(false,this);

                _userSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
