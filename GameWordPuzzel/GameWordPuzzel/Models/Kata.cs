using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{

    [TableName("Katas")]
    public class Kata:BaseNotify
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
              SetProperty(ref _id , value);
         
            }
        }

        [DbColumn("KategoriId")]
        public int KategoriId {
            get { return _kategoriid; }
            set
            {
               
                SetProperty(ref _kategoriid, value);
            }

        }

        [DbColumn("Nilai")]
        public string Nilai {
            get { return _nilai; }
            set
            {
                SetProperty(ref _nilai ,value);
            }
        }
    }
}
