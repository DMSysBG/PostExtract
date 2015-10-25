using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Data;
using DMSys.Systems;

namespace PostExtract
{
    public class PageExtract: IDisposable
    {
        private DBExtract _DBExtract = null;

        private int _IsDebug = 0;
        public int IsDebug
        {
            get
            { return _IsDebug; }
            set
            { _IsDebug = value; }
        }        

        public PageExtract(DBExtract dbExtract)
        {
            _DBExtract = dbExtract;
        }

        public void Dispose()
        { }

        public void Execute(PExtractTemplate peTemplate)
        {
            Console.WriteLine("Begin extract: {0}", peTemplate.SiteUrl);

            Console.WriteLine("Load Node Collection");
            LoadNodeCollection(peTemplate);

            if (_IsDebug == 0)
            {
                Console.WriteLine("Removes Duplicate");
                _DBExtract.RemovesDuplicate();
            }

            Console.WriteLine("Load Posts");
            LoadPosts(peTemplate);

            Console.WriteLine("Execute SQLAttributes");
            _DBExtract.ExecuteNonQuery(peTemplate.SQLAttributes);

            Console.WriteLine("Validate Publications");
            _DBExtract.ValidatePublications();

            Console.WriteLine("End extract: {0}", peTemplate.SiteUrl);
        }

        public void ExecutePost(string filePost, PExtractTemplate peTemplate)
        {
            Console.WriteLine("Begin extract: {0}", filePost);
            _DBExtract.EmptyNewPost();

            Console.WriteLine("Load Node Collection");
            int postId = _DBExtract.AddNewPost(peTemplate.SourceId, peTemplate.CategoryId, peTemplate.TemplateId, filePost, "");

            Console.WriteLine("Load Post");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.OptionDefaultStreamEncoding = Encoding.UTF8;
            htmlDoc.Load(filePost);            

            LoadPost(postId, htmlDoc, peTemplate);

            Console.WriteLine("Execute SQLAttributes");
            _DBExtract.ExecuteNonQuery(peTemplate.SQLAttributes);

            Console.WriteLine("Validate Publications");
            _DBExtract.ValidatePublications();

            Console.WriteLine("End extract: {0}", filePost);
        }
        
        public void ExecutePost(Uri uriPost, PExtractTemplate peTemplate)
        {
            Console.WriteLine("Begin extract: {0}", uriPost.ToString());
            _DBExtract.EmptyNewPost();
           
            Console.WriteLine("Load Post");
            int postId = -1;
            if ((_IsDebug == 0) || (_IsDebug >= 2))
            {
                postId = _DBExtract.AddNewPost(peTemplate.SourceId, peTemplate.CategoryId, peTemplate.TemplateId, uriPost.ToString(), "");
            }
            // Зарежда страницата
            HtmlWeb page = new HtmlWeb();
            HtmlDocument htmlDoc = page.Load(uriPost.ToString());

            LoadPost(postId, htmlDoc, peTemplate);

            if ((_IsDebug == 0) || (_IsDebug >= 3))
            {
                Console.WriteLine("Execute SQLAttributes");
                _DBExtract.ExecuteNonQuery(peTemplate.SQLAttributes);
            }
            if ((_IsDebug == 0) || (_IsDebug >= 4))
            {
                Console.WriteLine("Validate Publications");
                _DBExtract.ValidatePublications();
            }
            Console.WriteLine("End extract: {0}", uriPost.ToString());
        }

        /// <summary>
        /// Зарежда списък от публикации
        /// </summary>
        private void LoadNodeCollection(PExtractTemplate peList)
        {
            // Изтрива старите данни
            _DBExtract.EmptyNewPost();

            // Зарежда страницата
            HtmlWeb page = new HtmlWeb();
            HtmlDocument htmlDoc = page.Load(peList.SiteUrl);
            // Взема списъка от постове
            foreach (HtmlNode listItem in htmlDoc.DocumentNode.SelectNodes(peList.XPList))
            {
                // Обработва поста
                HtmlDocument htmlNode = new HtmlDocument();
                htmlNode.LoadHtml(listItem.OuterHtml);

                string link = SelectHtmlLink(htmlNode, peList.XPLink, peList.GetSiteHost());
                string image = SelectHtmlImage(htmlNode, peList.XPImage);

                // Записва данните
                _DBExtract.AddNewPost(peList.SourceId, peList.CategoryId, peList.TemplateId, link, image);
            }
        }

        private string SelectHtmlText(HtmlDocument htmlDoc, string xpText)
        {
            if (String.IsNullOrWhiteSpace(xpText))
            { return ""; }
            else
            {
                HtmlNode nText = htmlDoc.DocumentNode.SelectSingleNode(xpText);
                if (nText != null)
                { return nText.InnerText.Trim().Replace("\n", ";").Replace("\t", " "); }
                else
                { return ""; }
            }
        }

        private string SelectHtmlLink(HtmlDocument htmlDoc, string xpLink, string siteHost)
        {
            if (String.IsNullOrWhiteSpace(xpLink))
            { return ""; }
            else
            {
                HtmlNode linkItem = htmlDoc.DocumentNode.SelectSingleNode(xpLink);
                if (linkItem != null)
                {
                    HtmlAttribute attHref = linkItem.Attributes["href"];
                    if (attHref != null)
                    {
                        string link = attHref.Value.Split('#')[0];
                        if (link.StartsWith("http"))
                        { return link; }
                        else
                        { return siteHost + link; }
                    }
                    else
                    { return ""; }
                }
                else
                { return ""; }
            }
        }

        private string SelectHtmlImage(HtmlDocument htmlDoc, string xpImage)
        {
            if (String.IsNullOrWhiteSpace(xpImage))
            { return ""; }
            else
            {
                string image = "";
                HtmlNode imageItem = htmlDoc.DocumentNode.SelectSingleNode(xpImage);
                if (imageItem != null)
                {
                    HtmlAttribute attSrc = imageItem.Attributes["src"];
                    if (attSrc != null)
                    {
                        image = attSrc.Value;
                    }
                }
                return image;
            }
        }
        
        /// <summary>
        /// Зарежда публикациите
        /// </summary>
        private void LoadPosts(PExtractTemplate pePost)
        {
            if (!String.IsNullOrWhiteSpace(pePost.XPContent))
            {
                using (DataTable dtPosts = _DBExtract.GetNewPost())
                {
                    foreach (DataRow drPost in dtPosts.Rows)
                    {
                        int postId = TryParse.ToInt32(drPost["id"]);
                        string postLink = TryParse.ToString(drPost["post_link"]);

                        if (_IsDebug > 0)
                        { Console.WriteLine("{0}: {1}", postId, postLink); }

                        HtmlWeb page = new HtmlWeb();
                        HtmlDocument htmlDoc = page.Load(postLink);
                        LoadPost(postId, htmlDoc, pePost);
                    }
                }
            }
        }

        /// <summary>
        /// Зарежда данните от публикацията
        /// </summary>
        private void LoadPost(int postId, HtmlDocument htmlDoc, PExtractTemplate pePost)
        {            
            HtmlNode postContent = htmlDoc.DocumentNode.SelectSingleNode(pePost.XPContent);

            HtmlDocument htmlContent = new HtmlDocument();
            htmlDoc.OptionDefaultStreamEncoding = Encoding.UTF8;
            htmlContent.LoadHtml(postContent.InnerHtml);

            PPost pPost = new PPost();
            pPost.Title = SelectHtmlText(htmlContent, pePost.XPTitle);
            pPost.Text = SelectHtmlText(htmlContent, pePost.XPText);

            pPost.Images = new List<string>();
            if (!String.IsNullOrWhiteSpace(pePost.XPImages))
            {
                HtmlNodeCollection nodeImages = htmlDoc.DocumentNode.SelectNodes(pePost.XPImages);
                if (nodeImages != null)
                {
                    foreach (HtmlNode nImage in nodeImages)
                    {
                        HtmlAttribute attSrc = nImage.Attributes["src"];
                        pPost.Images.Add(attSrc.Value.Trim());
                    }
                }
            }

            pPost.Attributes = new List<string>();
            if (!String.IsNullOrWhiteSpace(pePost.XPAttributes))
            {
                HtmlNodeCollection nodeAttributes = htmlDoc.DocumentNode.SelectNodes(pePost.XPAttributes);
                if (nodeAttributes != null)
                {
                    foreach (HtmlNode nAttributes in nodeAttributes)
                    {
                        pPost.Attributes.Add(nAttributes.InnerText.Trim().Replace("\n", ";").Replace("\t", " "));
                    }
                }
            }

            pPost.Price = SelectHtmlText(htmlContent, pePost.XPPrice);
            pPost.Location = SelectHtmlText(htmlContent, pePost.XPLocation);
            pPost.Date = SelectHtmlText(htmlContent, pePost.XPDate);
            pPost.Posted = SelectHtmlText(htmlContent, pePost.XPPosted);
            if (_IsDebug > 0)
            {
                Console.WriteLine("Title    : {0}", pPost.Title); 
                Console.WriteLine("Text     : {0}", pPost.Text);
                Console.WriteLine("Price    : {0}", pPost.Price);
                Console.WriteLine("Location : {0}", pPost.Location);
                Console.WriteLine("Date     : {0}", pPost.Date);
                Console.WriteLine("Posted   : {0}", pPost.Posted);
            }
            if (postId > 0)
            {
                _DBExtract.SaveNewPost(postId, pePost.CategoryId, pePost.TemplateId, pPost);
            }
        }
    }
}