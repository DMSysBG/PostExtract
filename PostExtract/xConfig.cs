using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostExtract
{
    public class xConfig
    {
        public static string ConnectionString
        {
            get
            { return System.Configuration.ConfigurationManager.ConnectionStrings["posts"].ConnectionString; }
        } 
    }
}
