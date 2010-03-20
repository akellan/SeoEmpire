using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Web;
using NHibernate;
using SeoEmpire.Common;
using System.Drawing;
using System.IO;
using Timer=System.Timers.Timer;
using NHibernate.Criterion;
using System.Diagnostics;

namespace SeoEmpire.Utils
{
    public class PageParserManager
    {
        public Regex Title { get; set; }
        public Regex Text { get; set; }
        public Regex Date { get; set; }
        public Regex Images { get; set; }
        public Regex Description { get; set; }
        public Regex Keywords { get; set; }

        private const int Count = 20;
        private Timer[] _timers;
        private PageParser[] _parsers;

        
        public void Start()
        {
            _timers = new Timer[Count];
            _parsers = new PageParser[Count];

            for (int i = 0; i < Count; i++)
            {
                _parsers[i] = new PageParser
                                  {
                                      RTitle = Title,
                                      RText = Text,
                                      RImages = Images,
                                      RDescription = Description,
                                      RKeywords = Keywords,
                                      RDate = Date
                                  };
                
                Timer td = new Timer {Interval = 500};
                td.Elapsed += _parsers[i].InvokeParse;
                td.Start();

                _timers[i] = td;
            }
        }
    }

    public class PageParser
    {
        public Regex RTitle{ get; set;}
        public Regex RText{ get; set;}
        public Regex RDate{ get; set;}
        public Regex RImages{ get; set;}
        public Regex RDescription { get; set; }
        public Regex RKeywords { get; set; }

        public Match MTitle;
        public Match MText;
        public Match MDate;
        public MatchCollection MImages;

        public Uri PUri { get; set; }
        public string SavePath = @"output";

        public int _downloadErrorCount = 0;

        private object m_LockTimer = new object();
        
        public void InvokeParse(object sender, ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(m_LockTimer)) return;

            try
            {
                Parse(sender, e);
            }
            finally
            {
                Monitor.Exit(m_LockTimer);
            }

        }

        public void Parse(object sender, ElapsedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            lock (Crawler._qLock)
            {
                if (Crawler.Urls.Count <= 0)
                {
                    return;
                }

                PUri = Crawler.Urls.Dequeue();
            }


            /*if(IsExist(PUri))
            {
                return;
            }*/

            
            string content = null;

            int downloadErrorCount = 0;

            while (downloadErrorCount < 3 && string.IsNullOrEmpty(content))
            {

                try
                {
                    byte[] buff = new WebClient().DownloadData(PUri);

                    if (buff == null || buff.Length == 0)
                        throw new Exception("Buffer is null or empty");

                    //content = Encoding.UTF8.GetString(buff);
                    content = Encoding.GetEncoding(1251).GetString(buff);
                }
                catch { downloadErrorCount++; }    
            }

            if(string.IsNullOrEmpty(content))
            {
                return;
            }
            
            
            string articleTitle = RTitle.Match(content).Groups[1].Value;

            string articleText = TextUtils.StripTags(
                RText.Match(content).Groups[1].Value,
                new [] {"a","img"});

            DateTime articleDate = DateTime.MinValue;
            if (RDate != null)
            {
                try
                {
                    articleDate = DateTime.Parse(RDate.Match(content).Groups[1].Value);
                }catch { }
            }

            if (RImages != null)
                MImages = RImages.Matches(content);

            string description = String.Empty;
            if (RDescription != null)
                description = RDescription.Match(content).Groups[1].Value;

            string keywords = String.Empty;
            if (RKeywords != null)
                keywords = RKeywords.Match(content).Groups[1].Value;

            RuArticle ruArticle = new RuArticle
            {
                Date = articleDate,
                Title = articleTitle,
                Full = articleText,
                MenuId = 1,
                Short = String.Empty,
                Description = description,
                Keywords = keywords
            };

            //Вырезаем на время перевода елементы

            //TextReplacementsContainer enContainer = TextUtils.ReplaceTags(articleText, new string[] {"embed"});

            //TextReplacementsContainer ruContainer = (TextReplacementsContainer)enContainer.Clone();
            //TextReplacementsContainer ukContainer = (TextReplacementsContainer)enContainer.Clone();

            //string separator = " Wwwdeve <p> ";
            //string[] forTranslate = new[] {articleTitle, enContainer.Text, description, keywords};

            //string translateText = String.Join(separator, forTranslate);

            //string ruText = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(GoogleTranslate.Send(translateText,
            //                                                                                   Language.en, Language.ru)));

            //string uaText = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(GoogleTranslate.Send(translateText,
            //                                                                                   Language.en, Language.uk)));

            //string[] ruData = ruText.Split(new[] { "Wwwdeve" }, StringSplitOptions.RemoveEmptyEntries);
            //string[] uaData = uaText.Split(new[] { "Wwwdeve" }, StringSplitOptions.RemoveEmptyEntries);

            //ruContainer.Text = ruData[1];

            //RuArticle enArticle = new RuArticle
            //{
            //    Date = articleDate,
            //    Title = ruData[0] ,
            //    Full = TextUtils.UndoReplaceTags(ruContainer) ,
            //    MenuId = 1,
            //    Short = String.Empty,
            //    Description = ruData[2],
            //    Keywords = ruData[3]
            //};

            //ukContainer.Text = uaData[1];

            //UaArticle uaArticle = new UaArticle
            //{
            //    Date = articleDate,
            //    Title = uaData[0],
            //    Full = TextUtils.UndoReplaceTags(ukContainer),
            //    MenuId = 1,
            //    Short = String.Empty,
            //    Description = uaData[2],
            //    Keywords = uaData[3]
            //};

            

            //Save(enArticle);
            Save(ruArticle);
            //Save(uaArticle);

            sw.Stop();

            //Console.WriteLine("PageParser:new Article Title:{0} Text:{1} Time:{2}",
                //enArticle.Title.Length, enArticle.Full.Length, sw.Elapsed.TotalSeconds);

            sw.Start();

            foreach (Match mImage in MImages)
            {
                try
                {
                    GetImage(new Uri(mImage.Groups[1].Value)
                    , String.Format("{0}/{1}", SavePath, ruArticle.Id));    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PageParser:Image:{1}" + ex.Message);
                }
            }
            
            sw.Stop();
            Console.WriteLine("WithImages Time:{0}", sw.Elapsed.TotalSeconds);
        }

        private bool IsExist(Uri uri)
        {
            ISession session = DbDomain.CurrentSession;

            IList<EnArticle> result = session.CreateCriteria<EnArticle>()
                .Add(Restrictions.Eq("Url", uri.OriginalString))
                .List<EnArticle>();

            session.Flush();
            return result != null && result.Count > 0;
        }

        public void Save(object article)
        {
            ISession session = DbDomain.CurrentSession;
            using(ITransaction tr = session.BeginTransaction())
            {
                try
                {
                    session.Save(article);
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    Console.WriteLine("PageParser: MySql-{0}", ex.InnerException.Message);
                }
                tr.Commit();
            }
        }

        public void GetImage(Uri path, string savePath)
        {
            int errorCount = 0;
            byte[] img = null;

            while (errorCount < 3 && img == null)
            {
                try { img = new WebClient().DownloadData(path); }
                catch { errorCount++; }
            }

            if(img == null) return;

            string fileName = Path.GetFileName(path.OriginalString);

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            File.WriteAllBytes(savePath + "/" + fileName, img);
        }
    }
}
