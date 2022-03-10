using PanacheSoftware.Core.Domain.API.Automation;
using System;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Automation.Manager
{
    public interface IDocumentManager
    {
        Task<Tuple<bool, string>> AutomateDocument(AutoDoc autoDoc, string accessToken);
    }
}
