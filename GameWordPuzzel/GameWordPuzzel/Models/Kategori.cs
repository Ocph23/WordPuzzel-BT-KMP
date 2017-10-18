using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{

   [TableName("Kategori")]
   public class Kategori
    {
        [PrimaryKey("Id")]
        [DbColumn("Id")]
        public int Id { get; set; }

        [DbColumn("Name")]
        public string Name { get; set; }


        public List<Kata> DaftarKata { get; set; }
    }
}
