using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Post.Models;

namespace PostExtract
{
    public class PostPublish : IDisposable
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

        public PostPublish(DBExtract dbExtract)
        {
            _DBExtract = dbExtract;
        }

        public void Dispose()
        { }

        public void PublishPost(int postId)
        {
            string jsonPost = "";
            if ((_IsDebug == 0) || (_IsDebug >= 1))
            {
                if (_IsDebug != 0)
                { Console.WriteLine("Зарежда публикация"); }
                PostModel model = _DBExtract.GetPost(postId);
                if (model == null)
                {
                    Console.WriteLine("Публикация '{0}' не е открита", postId);
                    return;
                }

                if (_IsDebug != 0)
                { Console.WriteLine("Сериализира публикация"); }
                jsonPost = DMSys.Systems.ObjectJsonSerializer.Serialize<PostModel>(model);
                if (_IsDebug != 0)
                { Console.WriteLine("Публикация: " + jsonPost); }
            }

            string addPostUrl = xConfig.PublishUrl_AddPost;
            if (!String.IsNullOrWhiteSpace(jsonPost)
             && !String.IsNullOrWhiteSpace(addPostUrl))
            {
                using (var client = new System.Net.WebClient())
                {
                    var values = new System.Collections.Specialized.NameValueCollection();
                    values["model"] = jsonPost;
                    var response = client.UploadValues(addPostUrl, values);

                    var responseString = Encoding.Default.GetString(response);
                    if (_IsDebug != 0)
                    { Console.WriteLine("Upload Post: " + responseString); }
                }
            }
        }
    }
}
