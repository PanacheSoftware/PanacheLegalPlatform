//using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PanacheSoftware.Http
{
    public class PatchContent : StringContent
    {
        public PatchContent(object value)
            : base (JsonSerializer.Serialize(value), Encoding.UTF8,
                      "application/json-patch+json")
            //: base(JsonConvert.SerializeObject(value), Encoding.UTF8,
            //          "application/json-patch+json")
        {
        }
    }
}
