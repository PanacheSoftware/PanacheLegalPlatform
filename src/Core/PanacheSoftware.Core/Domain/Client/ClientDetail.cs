using PanacheSoftware.Core.Domain.Core;
using System;

namespace PanacheSoftware.Core.Domain.Client
{
    public class ClientDetail : PanacheSoftwareEntity
    {
        public Guid ClientHeaderId { get; set; }
        public virtual ClientHeader ClientHeader { get; set; }
        public string url { get; set; }
        public string Base64Image { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
