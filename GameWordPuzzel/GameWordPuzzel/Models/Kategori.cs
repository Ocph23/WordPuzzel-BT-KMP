using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{

   [TableName("Kategori")]
   public class Kategori:BaseNotify
    {
        private int? _id;
        private string _name;

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

        [DbColumn("Name")]
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }


        public List<Kata> Words { get; set; }
    }
}
