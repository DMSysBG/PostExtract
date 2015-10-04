using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostExtract
{
    public class PExtractTemplate
    {
        public int SourceId { get; set; }

        public int CategoryId { get; set; }

        public int TemplateId { get; set; }

        public string SiteUrl { get; set; }

        public string XPList { get; set; }

        public string XPLink { get; set; }

        public string XPImage { get; set; }

        public string XPContent { get; set; }

        public string XPTitle { get; set; }

        public string XPText { get; set; }

        public string XPImages { get; set; }

        public string XPAttributes { get; set; }

        public string XPPrice { get; set; }

        public string XPLocation { get; set; }

        public string XPDate { get; set; }

        public string XPPosted { get; set; }        

        public string SQLAttributes { get; set; }

        public string GetSiteHost()
        {
            if (String.IsNullOrWhiteSpace(this.SiteUrl))
            { return ""; }
            else
            { 
                Uri siteUrl = new Uri(this.SiteUrl);
                return siteUrl.AbsoluteUri.Substring(0, siteUrl.AbsoluteUri.Length - siteUrl.PathAndQuery.Length);
            }
        }
    }
}
