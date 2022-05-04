using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI.DataTable
{
    public class DataTableHeader
    {
        public DataTableHeader(string fieldName, string fieldTitle, bool allowSort = true)
        {
            FieldName = fieldName;
            FieldTitle = fieldTitle;
            AllowSort = allowSort;
        }

        public string FieldName { get; }

        public string FieldTitle { get; }

        public bool AllowSort { get; }
    }
}
