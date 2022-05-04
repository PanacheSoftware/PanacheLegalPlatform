using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI
{
    public class DocAutoCustomFieldTagList
    {
        public DocAutoCustomFieldTagList()
        {
            DocAutoCustomFieldTags = new List<DocAutoCustomFieldTag>();
        }

        public List<DocAutoCustomFieldTag> DocAutoCustomFieldTags { get; set; }
    }

    public class DocAutoCustomFieldTag
    {
        public string FieldName { get; set; }
        public string Tag { get; set; }
    }
}
