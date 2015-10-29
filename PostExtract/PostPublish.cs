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
            PostModel model = _DBExtract.GetPost(postId);

            Console.WriteLine(model.PText);
        }
    }
}
