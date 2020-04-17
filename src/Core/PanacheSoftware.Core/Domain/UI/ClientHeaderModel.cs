using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Client;
using PanacheSoftware.Core.Domain.API.Language;

namespace PanacheSoftware.Core.Domain.UI
{
    public class ClientHeaderModel
    {
        public ClientHead clientHead { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList TitleList { get; set; }
        public SelectList AddressTypeList { get; set; }
        public string contactCardHTML { get; set; }
        public string[] addressRows { get; set; }
        public LangQueryList langQueryList { get; set; }
    }
}
