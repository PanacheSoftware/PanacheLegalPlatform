using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.API.Automation
{
    public class AutoDoc
    {
        public AutoDoc()
        {
            AutoValues = new List<AutoVal>();
            OpenVariableSymbol = "{{$";
            CloseVariableSymbol = "$}}";
        }

        public byte[] Content { get; set; }
        public Guid FileHeaderId { get; set; }
        public Guid FileVersionId { get; set; }
        public string OpenVariableSymbol { get; set; }
        public string CloseVariableSymbol { get; set; }
        public string FileType { get; set; }
        public string TrustedName { get; set; }
        public IList<AutoVal> AutoValues { get; set; }
    }
}
