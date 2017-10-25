using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{

    [TableName("Katas")]
    public class Kata:DAL.BaseNotifyProperty
    {
        private int? _id;
        private int _kategoriid;
        private string _nilai;

        [PrimaryKey("Id")]
        [DbColumn("Id")]
        public int? Id {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        [DbColumn("KategoriId")]
        public int KategoriId {
            get { return _kategoriid; }
            set
            {
                _kategoriid = value;
               OnPropertyChanged();
            }

        }

        [DbColumn("Nilai")]
        public string Nilai {
            get { return _nilai; }
            set
            {
                _nilai= value;
                OnPropertyChanged();
            }
        }
    }
}
