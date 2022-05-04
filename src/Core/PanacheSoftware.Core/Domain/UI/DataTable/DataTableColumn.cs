using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI.DataTable
{
    public class DataTableColumn
    {
        public DataTableColumn(string value, string linkValue = default)
        {
            Value = value;
            LinkValue = linkValue;
        }

        public string Value { get; }

        public string LinkValue { get; }

        public bool IsLink { get => !string.IsNullOrWhiteSpace(LinkValue); }
    }
}
