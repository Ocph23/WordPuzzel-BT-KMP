﻿using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{
    [TableName("Pilihan")]
  public  class Option:BaseNotifyProperty
    {
              
        private int? _id;
        private string _value;
        private int _soalId;

        [PrimaryKey("Id")]
        [DbColumn("Id")]

        public int? Id
        {
            get { return _id; }
            set { _id = value.Value;OnPropertyChanged(); }
        }

       
        [DbColumn("SoalId")]

        public int SoalId
        {
            get { return _soalId; }
            set { _soalId = value; OnPropertyChanged(); }
        }

        
        [DbColumn("Value")]
        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }


        private bool _isTrue;
        private bool _userSelected;
        internal delChangeAnswer delAnswer;

        [DbColumn("IsTrue")]
        public bool IsTrueAnswer
        {
            get { return _isTrue; }
            set { _isTrue = value;OnPropertyChanged(); }
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
