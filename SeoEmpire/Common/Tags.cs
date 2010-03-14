using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;

namespace SeoEmpire.Common
{
    public class Tag
    {
        public virtual int Id{ get; set;}
        public virtual string Name { get; set; }
    }

    public class UaTag : Tag
    { }

    public class RuTag : Tag
    { }

    public class EnTag : Tag
    { }
}
