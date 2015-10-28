using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMSys.Systems;

namespace PostExtract
{
    /*
    /d: 0 - Release
        1 - Принтира публикацита на конзолата
        2 - Добавя поста в базата
        3 - Изпълнява SQL от базата
        4 - Валидира публикацията
        5 - Transfer
    */
    class Program
    {
        static void Main(string[] args)
        {
            // Console.OutputEncoding = Encoding.Unicode;
            // Режим на Debug
            int isDebug = 0;
            // Id на източник
            int sourceId = 0;
            // Id на публикация за публикуване
            int publishPostId = 0;
            // Url: Публикация
            string urlPost = "";
            // Файл: Публикация
            string filePost = "";
            
            foreach (string arg in args)
            {
                int argIndex = arg.IndexOf(':');
                string argKey = arg.Substring(0, argIndex);
                string argValue = arg.Substring(argIndex + 1, arg.Length - argIndex - 1);
                switch (argKey)
                {
                    case "/d":  // Режим на Debug
                        isDebug = TryParse.ToInt32(argValue);
                        break;
                    case "/s":  // Id на източник
                        sourceId = TryParse.ToInt32(argValue);
                        break;
                    case "/up": // Адрес на публикация
                        urlPost = TryParse.ToString(argValue);
                        break;
                    case "/fp": // Път на публикация
                        filePost = TryParse.ToString(argValue);
                        break;
                    case "/ppi":  // Id на публикация за публикуване
                        publishPostId = TryParse.ToInt32(argValue);
                        break;
                }
            }
            // Изпълнява се само за sourceId
            if (sourceId > 0)
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

                                // Изпълнява се само за публикации на файла
                                if (!String.IsNullOrWhiteSpace(filePost))
                                {
                                    pExtract.ExecutePost(filePost, peTemplate);
                                }
                                // Изпълнява се само за публикации от адреса
                                else if (!String.IsNullOrWhiteSpace(urlPost))
                                {
                                    pExtract.ExecutePost(new Uri(urlPost), peTemplate);
                                }
                                // за всички публикации от източника
                                else
                                {
                                    pExtract.Execute(peTemplate);
                                }                                
                            }
                            if ((isDebug == 0) || (isDebug >= 5))
                            {
                                dbExtract.TransferNewPost();
                            }
                            if (isDebug == 0)
                            {
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
            // Изпълнява се само за publishPostId
            else if (publishPostId > 0)
            {
                try
                {
                    using (DBExtract dbExtract = new DBExtract())
                    {
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
                            if (isDebug == 0)
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
            if (isDebug > 0)
            {
                Console.ReadKey();
            }
        }
    }
}