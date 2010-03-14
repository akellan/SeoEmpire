using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Google.API.Translate;
using SeoEmpire.Common;
using NHibernate;
using SeoEmpire.Utils;
using Language=SeoEmpire.Utils.Language;
using HtmlAgilityPack;
using System.Xml;
using System.Net;
using System.IO;

namespace SeoEmpire
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            DbDomain.InitSqLite();

            ISession session = DbDomain.CurrentSession;

            News news2 = session.Get<News>(2);

            ITransaction tr = session.BeginTransaction();

            News news = new News
                            {
                                Body = "Body",
                                Date = DateTime.Now,
                                Image = new byte[] {1, 2, 3},
                                Title = "Title"
                            };

            session.Save(news);
            int newsId = news.Id;

            tr.Commit();
            session.Flush();

            News news1 = session.Get<News>(newsId);
            
            DbDomain.Close();
             */

            /*DbDomain.InitMySql();

            ISession session = DbDomain.CurrentSession;

            //ITransaction tr = session.BeginTransaction();

            Article news = new Article
                               {
                                   Date = DateTime.Now,
                                   Title = "Fuck You",
                                   Full = "Кроме того, результаты последних исследований подтверждают, что эффективными для лечения и профилактики свиного гриппа являются такие дорогостоящие противовирусные препараты как Занамивир (торговое название Relenza), Озельтамивир (торговое название Tamiflu или Ribavirin), Аматадин (торговые названия  Глудантан, Мидантан, ПК-Мерц) и российский препарат Ремантадин. Однако, следует помнить, что перед началом приема этих препаратов необходима консультация врача.",
                                   MenuId = 1,
                                   Short = "Short"

            };

            news.Full = GoogleTranslate.Send(news.Full, Language.ru, Language.en);
            //TranslateClient client = new TranslateClient("http://localhost");
            //news.Full = client.Translate(news.Full, Language.Russian,  Language.English);
            

            session.Save(news);
            int newsId = news.Id;

            //tr.Commit();
            session.Flush();

            Article news1 = session.Get<Article>(newsId);

            DbDomain.Close();*/
            
            DbDomain.InitMySql();

            //FssDbUtils.GenerateDayMonthYearFields<EnArticle>();
            //FssDbUtils.GenerateDayMonthYearFields<RuArticle>();
            //FssDbUtils.GenerateDayMonthYearFields<UaArticle>();

            //FssDbUtils.GenerateShort<EnArticle>();
            //FssDbUtils.GenerateShort<RuArticle>();
            //FssDbUtils.GenerateShort<UaArticle>();

            /*FssDbUtils.GoogleTranslateClosedTagsFix<RuArticle>();
            FssDbUtils.GoogleTranslateClosedTagsFix<UaArticle>();*/

            /*FssDbUtils.GenerateTags<EnTag, EnArticle>();

            FssDbUtils.TranslateTags<EnTag, RuTag>(Language.en, Language.ru);
            FssDbUtils.TranslateTags<EnTag, UaTag>(Language.en, Language.uk);*/

            /*FssDbUtils.GenerateTagRelationships<EnTag,EnArticle,EnTagsRel>();*/
            

            /*

            Crawler crawler = new Crawler(
                //new Uri("http://allworldcars.com/wordpress/"),
                new Uri("http://allworldcars.com/wordpress/index.php?paged=550"),
                String.Empty,

                new Regex(@"href=""(http://allworldcars.com/wordpress/index.php\?paged=[\d]{1,5})"">Next Page &raquo;</a>",
                        RegexOptions.Compiled),

                new Regex(@"<a href=""(http://allworldcars.com/wordpress/\?p=[\d]{1,6})"" rel=""bookmark""",
                        RegexOptions.Compiled),
                        150
                );
            crawler.Start();

            PageParserManager parserManager = new PageParserManager();

            parserManager.Title = 
                new Regex(@"<a href=""[^""]+"" rel=""bookmark""[^>]+>([\s\S]+?)</a>", 
                        RegexOptions.Compiled);

            parserManager.Text = 
                new Regex(@"<div class=""post-content"" style=""clear: both;"">([\s\S]+?)<div class=""post-info"">", 
                        RegexOptions.Compiled);

            parserManager.Images = 
                new Regex(@"<div class=""post-content"" style=""clear: both;"">[\s\S]+?<img src=""([^""]+?)""",
                    RegexOptions.Compiled);
            
            parserManager.Description = new Regex(@"<meta name=""description"" content=""([^""]+?)""", 
                    RegexOptions.Compiled);

            parserManager.Keywords = new Regex(@"<meta name=""keywords"" content=""([^""]+?)""",
                    RegexOptions.Compiled);

            parserManager.Date = new Regex(@"<p class=""post-date"">([^<]+)</p>",
                    RegexOptions.Compiled);

            parserManager.Start();*/
			
			
            WebClient client = new WebClient();

            client.DownloadFile(new Uri("http://www.domik.net/mod/main/news/"),"test.tmp");

            

            //HtmlWeb test = new HtmlWeb();
            //HtmlDocument doc = test.Load("http://www.domik.net/mod/main/news/");

            HtmlDocument doc = new HtmlDocument();

            doc.Load("test.tmp", Encoding.GetEncoding(1251));

                       
						
			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//title");
			
			
			
			
			

            Console.WriteLine("End ...");

            Console.ReadKey();
            
            DbDomain.Close();
        }
    }
}
