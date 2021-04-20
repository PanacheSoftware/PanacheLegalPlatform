using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI.DataTable
{
    public class DataTableRow
    {
        public DataTableRow()
        {
            DataTableColumns = new List<DataTableColumn>();
        }

        public List<DataTableColumn> DataTableColumns { get; set; }
    }
}
