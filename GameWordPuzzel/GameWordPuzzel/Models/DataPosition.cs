using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{
    public class DataPosition
    {

        public DataPosition(string item)
        {
            this.Content = item;
            Datas = new List<ButtonView>();
        }

        public List<ButtonView> Datas { get; set; }
        public Cordinate From { get; set; }
        public Cordinate To { get; set; }
        public string Content { get; set; }
    }
}
