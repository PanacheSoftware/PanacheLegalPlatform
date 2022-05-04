using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Automation
{
    public class AutoVal
    {
        public AutoVal(string placeholder, string value)
        {
            Placeholder = placeholder;
            Value = value;
        }

        public string Placeholder { get; set; }
        public string Value { get; set; }
    }
}
