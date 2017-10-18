using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{
   public class Kata
    {
        public int Id { get; set; }
        public int KategoriId { get; set; }
        public string Value { get; set; }
    }
}
