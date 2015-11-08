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

        public static string PublishHost
        {
            get
            { return System.Configuration.ConfigurationManager.AppSettings["PUBLISH_HOST"]; }
        }

        public static string PublishUrl_AddPost
        {
            get
            {
                string publishHost = PublishHost;
                if (String.IsNullOrWhiteSpace(publishHost))
                { return ""; }
                else
                { return System.IO.Path.Combine(publishHost, "Post/Add"); }
            }
        }
    }
}
