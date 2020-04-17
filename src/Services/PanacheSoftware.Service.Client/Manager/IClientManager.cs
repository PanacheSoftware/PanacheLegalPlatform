using PanacheSoftware.Core.Domain.API.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Client.Manager
{
    public interface IClientManager
    {
        ClientSummary GetClientSummary(string id);
    }
}
