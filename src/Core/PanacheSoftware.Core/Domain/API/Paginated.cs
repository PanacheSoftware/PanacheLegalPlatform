using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API
{
    public class Paginated<T>
    {
        public Paginated()
        {
            Items = new List<T>();
            PageIndex = 0;
            TotalPages = 0;
        }

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; }

        public Paginated(List<T> items, int count, int pageIndex, int pageSize, int? totalPages)
        {
            PageIndex = pageIndex;

            if(totalPages.HasValue)
            {
                TotalPages = totalPages.Value;
            }
            else
            {
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            }
            
            Items = new List<T>();
            Items.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
            set { }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
            set { }
        }
    }
}
