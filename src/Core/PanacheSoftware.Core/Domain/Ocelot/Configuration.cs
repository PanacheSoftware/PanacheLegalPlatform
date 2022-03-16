using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanacheSoftware.Core.Domain.Ocelot
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    //public class AddHeadersToRequest
    //{
    //}

    //public class UpstreamHeaderTransform
    //{
    //}

    //public class DownstreamHeaderTransform
    //{
    //}

    //public class AddClaimsToRequest
    //{
    //}

    //public class RouteClaimsRequirement
    //{
    //}

    //public class AddQueriesToRequest
    //{
    //}

    //public class ChangeDownstreamPathTemplate
    //{
    //}

    //public class FileCacheOptions
    //{
    //    public int ttlSeconds { get; set; }
    //    public object region { get; set; }
    //}

    //public class QoSOptions
    //{
    //    public int exceptionsAllowedBeforeBreaking { get; set; }
    //    public int durationOfBreak { get; set; }
    //    public int timeoutValue { get; set; }
    //}

    //public class LoadBalancerOptions
    //{
    //    public object type { get; set; }
    //    public object key { get; set; }
    //    public int expiry { get; set; }
    //}

    //public class RateLimitOptions
    //{
    //    public List<object> clientWhitelist { get; set; }
    //    public bool enableRateLimiting { get; set; }
    //    public object period { get; set; }
    //    public double periodTimespan { get; set; }
    //    public int limit { get; set; }
    //    public string clientIdHeader { get; set; }
    //    public object quotaExceededMessage { get; set; }
    //    public string rateLimitCounterPrefix { get; set; }
    //    public bool disableRateLimitHeaders { get; set; }
    //    public int httpStatusCode { get; set; }
    //}

    public class AuthenticationOptions
    {
        public AuthenticationOptions()
        {
            AllowedScopes = new List<object>();
        }

        public string AuthenticationProviderKey { get; set; }
        public List<object> AllowedScopes { get; set; }
    }

    //public class HttpHandlerOptions
    //{
    //    public bool allowAutoRedirect { get; set; }
    //    public bool useCookieContainer { get; set; }
    //    public bool useTracing { get; set; }
    //    public bool useProxy { get; set; }
    //    public object maxConnectionsPerServer { get; set; }
    //}

    public class DownstreamHostAndPort
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

    //public class SecurityOptions
    //{
    //    public List<object> ipAllowedList { get; set; }
    //    public List<object> ipBlockedList { get; set; }
    //}

    public class Route
    {
        public string DownstreamPathTemplate { get; set; } //Used
        public string UpstreamPathTemplate { get; set; } //Used
        public List<string> UpstreamHttpMethod { get; set; } //Used
        //public object downstreamHttpMethod { get; set; }
        //public AddHeadersToRequest addHeadersToRequest { get; set; }
        //public UpstreamHeaderTransform upstreamHeaderTransform { get; set; }
        //public DownstreamHeaderTransform downstreamHeaderTransform { get; set; }
        //public AddClaimsToRequest addClaimsToRequest { get; set; }
        //public RouteClaimsRequirement routeClaimsRequirement { get; set; }
        //public AddQueriesToRequest addQueriesToRequest { get; set; }
        //public ChangeDownstreamPathTemplate changeDownstreamPathTemplate { get; set; }
        //public object requestIdKey { get; set; }
        //public FileCacheOptions fileCacheOptions { get; set; }
        public bool RouteIsCaseSensitive { get; set; } //Used
        //public object serviceName { get; set; }
        //public object serviceNamespace { get; set; }
        public string DownstreamScheme { get; set; } //Used
        //public QoSOptions qoSOptions { get; set; }
        //public LoadBalancerOptions loadBalancerOptions { get; set; }
        //public RateLimitOptions rateLimitOptions { get; set; }
        public AuthenticationOptions AuthenticationOptions { get; set; } //Used
        //public HttpHandlerOptions httpHandlerOptions { get; set; }
        public List<DownstreamHostAndPort> DownstreamHostAndPorts { get; set; } //Used
        //public object upstreamHost { get; set; }
        //public object key { get; set; }
        //public List<object> delegatingHandlers { get; set; }
        //public int priority { get; set; }
        //public int timeout { get; set; }
        //public bool dangerousAcceptAnyServerCertificateValidator { get; set; }
        //public SecurityOptions securityOptions { get; set; }
        //public object downstreamHttpVersion { get; set; }
    }

    //public class ServiceDiscoveryProvider
    //{
    //    public object scheme { get; set; }
    //    public object host { get; set; }
    //    public int port { get; set; }
    //    public object type { get; set; }
    //    public object token { get; set; }
    //    public object configurationKey { get; set; }
    //    public int pollingInterval { get; set; }
    //    public object @namespace { get; set; }
    //}

    public class GlobalConfiguration
    {
        //public object requestIdKey { get; set; }
        //public ServiceDiscoveryProvider serviceDiscoveryProvider { get; set; }
        //public RateLimitOptions rateLimitOptions { get; set; }
        //public QoSOptions qoSOptions { get; set; }
        public string BaseUrl { get; set; }
        //public LoadBalancerOptions loadBalancerOptions { get; set; }
        //public object downstreamScheme { get; set; }
        //public HttpHandlerOptions httpHandlerOptions { get; set; }
        //public object downstreamHttpVersion { get; set; }
    }

    public class Root
    {
        public Root()
        {
            Routes = new List<Route>();
            //dynamicRoutes = new List<object>();
            //aggregates = new List<object>();
        }

        public List<Route> Routes { get; set; }
        //public List<object> dynamicRoutes { get; set; }
        //public List<object> aggregates { get; set; }
        public GlobalConfiguration GlobalConfiguration { get; set; }
    }


}
