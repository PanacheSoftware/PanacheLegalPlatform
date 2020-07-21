using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Configuration
{
    public class PanacheSoftwareConfiguration
    {
        public URLCallMethod CallMethod { get; set; } = new URLCallMethod();
        public URLConfiguration Url { get; set; } = new URLConfiguration();
        public SecretConfiguration Secret { get; set; } = new SecretConfiguration();
        public string StartDomain { get; set; }
        public string DBProvider { get; set; }
    }

    public class URLCallMethod
    {
        public string APICallsSecure { get; set; }
        public string UICallsSecure { get; set; }
        public string UseAPIGateway { get; set; }
    }

    public class URLConfiguration
    {
        public string IdentityServerURL { get; set; }
        public string IdentityServerURLSecure { get; set; }
        public string APIGatewayURL { get; set; }
        public string APIGatewayURLSecure { get; set; }
        public string UIClientURL { get; set; }
        public string UIClientURLSecure { get; set; }


        public string ClientServiceURL { get; set; }
        public string FileServiceURL { get; set; }
        public string FoundationServiceURL { get; set; }
        public string TaskServiceURL { get; set; }
        public string TeamServiceURL { get; set; }
        public string ClientServiceURLSecure { get; set; }
        public string FileServiceURLSecure { get; set; }
        public string FoundationServiceURLSecure { get; set; }
        public string TaskServiceURLSecure { get; set; }
        public string TeamServiceURLSecure { get; set; }
    }

    public class SecretConfiguration
    {
        public string UIClientSecret { get; set; }
        public string APIGatewaySecret { get; set; }
        public string ClientServiceSecret { get; set; }
        public string FileServiceSecret { get; set; }
        public string FoundationServiceSecret { get; set; }
        public string TaskServiceSecret { get; set; }
        public string TeamServiceSecret { get; set; }
    }
}
