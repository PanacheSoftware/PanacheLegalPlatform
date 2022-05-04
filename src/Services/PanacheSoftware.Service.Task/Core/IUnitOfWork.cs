using PanacheSoftware.Database.Core;
using PanacheSoftware.Service.Task.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Core
{
    public interface IUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        ITaskGroupHeaderRepository TaskGroupHeaders { get; }
        ITaskGroupDetailRepository TaskGroupDetails { get; }
        ITaskHeaderRepository TaskHeaders { get; }
        ITaskDetailRepository TaskDetails { get; }

        ITemplateDetailRepository TemplateDetails { get; }
        ITemplateGroupDetailRepository TemplateGroupDetails { get; }
        ITemplateGroupHeaderRepository TemplateGroupHeaders { get; }
        ITemplateHeaderRepository TemplateHeaders { get; }
        ITemplateItemDetailRepository TemplateItemDetails { get; }
        ITemplateItemHeaderRepository TemplateItemHeaders { get; }
    }
}
