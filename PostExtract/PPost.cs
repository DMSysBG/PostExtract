using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostExtract
{
    public class PPost
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public List<string> Images { get; set; }

        public List<string> Attributes { get; set; }

        public string Price { get; set; }

        public string Location { get; set; }

        public string Date { get; set; }

        public string Posted { get; set; }
    }
}
