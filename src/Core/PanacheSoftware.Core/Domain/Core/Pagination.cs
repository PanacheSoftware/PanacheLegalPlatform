using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Core
{
    public class Pagination
    {
        public Pagination(int? pageNumber, string sortField, string sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrders.Ascending:
                case SortOrders.Descending:
                    SortOrder = sortOrder;
                    break;
                default:
                    SortOrder = SortOrders.Ascending;
                    break;
            }

            PageNumber = 1;

            if (pageNumber.HasValue)
                PageNumber = pageNumber.Value <= 0 ? 1 : pageNumber.Value;


            PageNumber = pageNumber.HasValue ? pageNumber.Value : 1;
            SortField = string.IsNullOrWhiteSpace(sortField) ? string.Empty : sortField;
        }

        public string SortOrder { get; set; }
        public int PageNumber { get; set; }
        public string SortField { get; set; }
    }
}
