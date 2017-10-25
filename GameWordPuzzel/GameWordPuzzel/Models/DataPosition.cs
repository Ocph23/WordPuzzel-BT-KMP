using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWordPuzzel.Models
{
    public class DataPosition
    {
        StringBuilder sb = new StringBuilder();
        public DataPosition(string item)
        {
            sb.Append(item);
            Datas = new List<ButtonView>();
        }
        public DataPosition()
        {
            Datas = new List<ButtonView>();
        }

        public List<ButtonView> Datas { get; set; }
        public Cordinate From { get; set; }
        public Cordinate To { get; set; }
        public string Content { get { return sb.ToString(); } }

       
        internal void AddButton(ButtonView btn)
        {
            Datas.Add(btn);
            sb.Append(btn.main.Content.ToString());
        }
    }
}
