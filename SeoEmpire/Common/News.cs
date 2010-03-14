using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeoEmpire.Common
{
    public class News
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Body { get; set; }
        public virtual byte[] Image { get; set; }
    }
}
