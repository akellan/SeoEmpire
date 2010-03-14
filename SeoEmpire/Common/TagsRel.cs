using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeoEmpire.Common
{
    public class TagsRel
    {
        public virtual int Id { get; set; }
        public virtual int ArtId { get; set; }
        public virtual int TagId { get; set; }
    }

    public class EnTagsRel : TagsRel
    { }

    public class RuTagsRel : TagsRel
    { }

    public class UaTagsRel : TagsRel
    { }
}
