using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.UI.DataTable
{
    public class CreateButton
    {
        public CreateButton(string title, string linkValue)
        {
            Title = title;
            LinkValue = linkValue;
        }

        public string Title { get; }
        public string LinkValue { get; }
    }
}
