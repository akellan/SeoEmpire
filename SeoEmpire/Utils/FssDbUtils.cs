using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using NHibernate;
using SeoEmpire.Common;
using NHibernate.Criterion;
using System.Web;
using System.Text.RegularExpressions;

namespace SeoEmpire.Utils
{
    static class FssDbUtils
    {
        /// <summary>
        /// Исправляем баг с тегами в гугл переводе
        /// </summary>
        /// <typeparam name="T"></typeparam>
        static public void GoogleTranslateClosedTagsFix<T>()
            where T : Article
        {
            ISession session = DbDomain.CurrentSession;

            IList<T> articles = session.CreateCriteria<T>()
                .AddOrder(Order.Asc("Id"))
                .List<T>();

            foreach (T article in articles)
            {
                article.Full = GoogleTranslate.GoogleTranslateClosedTagsFastHack(article.Full);
                //session.Save(article);
            }

            session.Flush();
        }

        /// <summary>
        /// Генерация тегов
        /// </summary>
        /// <typeparam name="Tags">Теги</typeparam>
        /// <typeparam name="Articles">Статьи</typeparam>
        static public void GenerateTags<Tags, Articles>()
            where Tags : Tag, new()
            where Articles : Article
        {

            string[][] nonuniqueUtf8Symbols = new string[][]
                                                  {
                                                      new string[] {"ä","a"},
                                                      new string[] {"ö","o"},
                                                      new string[] {"ü","u"},
                                                      new string[] {"ß","ss"},
                                                  };

            ISession session = DbDomain.CurrentSession;

            IList<Articles> articles = session.CreateCriteria<Articles>()
                .AddOrder(Order.Asc("Id"))
                .List<Articles>();

            Dictionary<string,Tags> tags = new Dictionary<string, Tags>();

            foreach (Articles article in articles)
            {
                string[] keywords = article.Keywords.Split(new []{","},
                    StringSplitOptions.RemoveEmptyEntries);

                if(keywords.Length > 0)
                {
                    foreach (string keyword in keywords)
                    {
                        string clearKeyword = keyword.Trim().ToLower();

                        foreach (string[] s in nonuniqueUtf8Symbols)
                            clearKeyword = clearKeyword.Replace(s[0], s[1]);

                        if (!tags.ContainsKey(clearKeyword))
                        {
                            tags.Add(clearKeyword, new Tags()
                                                  {
                                                      Name = clearKeyword
                                                  });
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, Tags> tag in tags)
            {
                //tag.Value.InsertIgnoreIntoSession();
                if(session.CreateCriteria<Tags>().Add(Restrictions.Eq("Name",tag.Value.Name)).SetMaxResults(1).List().Count == 0)
                    session.Save(tag.Value);
            }
            
            session.Flush();
        }

        static public void TranslateTags<TagsFrom,TagsTo>(Language lngFrom, Language lngTo)
            where TagsFrom : Tag, new()
            where TagsTo : Tag, new()
        {
            ISession session = DbDomain.CurrentSession;
            IList<TagsFrom> tags = session.CreateCriteria<TagsFrom>()
               .AddOrder(Order.Asc("Id"))
               .List<TagsFrom>();
            
            foreach (TagsFrom tag in tags)
            {
                session.Save(new TagsTo()
                                 {
                                     Id = tag.Id,
                                     Name = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(GoogleTranslate.Send(tag.Name, lngFrom, lngTo))).ToLower()
                                 });
            }
            
            session.Flush();
        }

        public static void GenerateTagRelationships<Tags, Articles, TagsRels>()
            where Tags : Tag, new()
            where Articles : Article, new()
            where TagsRels : TagsRel, new()
        {
            ISession session = DbDomain.CurrentSession;


            IList<Tags> tags = session.CreateCriteria<Tags>()
               .AddOrder(Order.Asc("Id"))
               .List<Tags>();

            foreach (Tags tag in tags)
            {
                foreach(Articles article in session.CreateCriteria<Articles>()
                    .Add(Restrictions.Or(Restrictions.Like("Keywords", tag.Name, MatchMode.Anywhere),
                        Restrictions.Or(Restrictions.Like("Full", tag.Name, MatchMode.Anywhere), Restrictions.Like("Title", tag.Name, MatchMode.Anywhere))
                    )).List<Articles>())
                {
                    session.Save(new TagsRels
                                     {
                                         ArtId = article.Id,
                                         TagId = tag.Id
                                     });
                }
            }

            session.Flush();
        }


        static public void GenerateDayMonthYearFields<Articles>()
            where Articles : Article
        {
            ISession session = DbDomain.CurrentSession;

            IList<Articles> tags = session.CreateCriteria<Articles>().List<Articles>();

            foreach (Articles tag in tags)
            {
                tag.Date = tag.Date.AddYears(1);
                tag.Day = tag.Date.Day;
                tag.Month = tag.Date.Month;
                tag.Year = tag.Date.Year;
            }

            session.Flush();
        }

        static public void GenerateShort<TArticle>()
            where TArticle : Article
        {
            ISession session = DbDomain.CurrentSession;

            foreach (TArticle tag in session.CreateCriteria<TArticle>().List<TArticle>())
            {
                string strippedText = TextUtils.StripAllTags(tag.Full).Trim();

                if(strippedText.Length < 200)
                    tag.Short = strippedText;
                else
                    tag.Short = String.Format("{0}...",strippedText.Substring(0, 200));
            }

            session.Flush();
        }
    }
}
