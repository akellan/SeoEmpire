using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeoEmpire.Common
{
    public class Article
    {
        public virtual Int32 Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual String Description { get; set; }
        public virtual String Keywords { get; set; }
        public virtual String Short { get; set; }
        public virtual String Full { get; set; }
        public virtual String Title { get; set; }
        public virtual Int32 MenuId { get; set; }
        public virtual Int32 Day { get; set; }
        public virtual Int32 Month { get; set; }
        public virtual Int32 Year { get; set; }
    }

    public class RuArticle : Article
    { }

    public class EnArticle : Article
    {
        public virtual string Url { get; set; }
    }

    public class UaArticle : Article
    { }
}
