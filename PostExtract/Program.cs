using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMSys.Systems;

namespace PostExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            // Режим на Debug
            bool isDebug = false;
            // Id на източник
            int sourceId = 0;
            // Файл: Списък от публикации
            string fileCollection = "";
            // Файл: Публикация
            string filePost = "";
            
            foreach (string arg in args)
            {
                string[] value = arg.Split(':');
                switch (value[0])
                {
                    case "/d":
                        isDebug = true;
                        break;
                    case "/s":
                        if (value.Length == 2)
                        { sourceId = TryParse.ToInt32(value[1]); }
                        break;
                    case "/fc":
                        if (value.Length == 2)
                        { fileCollection = TryParse.ToString(value[1]); }
                        break;
                    case "/fp":
                        if (value.Length == 2)
                        { filePost = TryParse.ToString(value[1]); }
                        break;
                }
            }
            if (!String.IsNullOrWhiteSpace(filePost))
            {
                try
                {
                    using (DBExtract dbExtract = new DBExtract())
                    {
                        PExtractTemplate peTemplate = dbExtract.GetPETemplate(sourceId);
                        if (peTemplate == null)
                        {
                            Console.WriteLine("Source '{0}' not found", sourceId);
                        }
                        else
                        {
                            using (PageExtract pExtract = new PageExtract(dbExtract))
                            {
                                pExtract.IsDebug = isDebug;

                                pExtract.ExecutePost(filePost, peTemplate);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            // Изпълнява се само за sourceId
            else if (sourceId > 0)
            {
                try
                {
                    using (DBExtract dbExtract = new DBExtract())
                    {
                        PExtractTemplate peTemplate = dbExtract.GetPETemplate(sourceId);
                        if (peTemplate == null)
                        {
                            Console.WriteLine("Source '{0}' not found", sourceId);
                        }
                        else
                        {
                            using (PageExtract pExtract = new PageExtract(dbExtract))
                            {
                                pExtract.IsDebug = isDebug;

                                pExtract.Execute(peTemplate);
                            }
                            if (!isDebug)
                            {
                                dbExtract.TransferNewPost();
                                dbExtract.EmptyNewPost();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            // За всички sources
            else
            {
                try
                {
                    using (DBExtract dbExtract = new DBExtract())
                    {
                        List<PExtractTemplate> peTemplates = dbExtract.GetPETemplates();
                        foreach (PExtractTemplate peTemplate in peTemplates)
                        {
                            using (PageExtract pExtract = new PageExtract(dbExtract))
                            {
                                pExtract.IsDebug = isDebug;

                                pExtract.Execute(peTemplate);
                            }
                            if (!isDebug)
                            {
                                dbExtract.TransferNewPost();
                                dbExtract.EmptyNewPost();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            Console.WriteLine("Complete");
            if (isDebug)
            {
                Console.ReadKey();
            }
        }
    }
}
