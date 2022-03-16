using PanacheSoftware.Core.Domain.API.Task.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Manager
{
    public interface ITemplateManager
    {
        TemplateHeadList GetTemplateHeadList();
        Task<Tuple<Guid, string>> CreateTemplateFromTask(TemplateHead templateHead, Guid TaskGroupHeaderId, string accessToken);
    }
}
