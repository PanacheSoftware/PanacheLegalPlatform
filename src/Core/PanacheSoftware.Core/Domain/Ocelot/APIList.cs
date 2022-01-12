using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Ocelot
{
    public class APIList
    {
        public APIList()
        {
            APIListDetails = new List<APIListDetail>();
        }

        public List<APIListDetail> APIListDetails { get; set; }
    }

    public class APIListDetail
    {
        public APIListDetail(string BaseURI, string GetPart, string swaggerVer = "v1")
        {
            APIBaseURI = new Uri(BaseURI);
            SwaggerVersion = swaggerVer;
            APIGetPart = string.IsNullOrWhiteSpace(GetPart) ? $"/swagger/{SwaggerVersion}/swagger.json" : GetPart;
        }

        public Uri APIBaseURI { get; }
        public string APIGetPart { get; }
        public string SwaggerVersion { get; }
        public string HttpPart { get => APIBaseURI.Scheme; }
        public string HostPart { get => APIBaseURI.Host; }
        public int PortPart { get => APIBaseURI.Port; }
        
    }
}
