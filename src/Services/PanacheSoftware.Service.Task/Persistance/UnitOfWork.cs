using PanacheSoftware.Database.Persistance;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core;
using PanacheSoftware.Service.Task.Core.Repositories;
using PanacheSoftware.Service.Task.Persistance.Context;
using PanacheSoftware.Service.Task.Persistance.Repositories.Task;
using PanacheSoftware.Service.Task.Persistance.Repositories.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance
{
    public class UnitOfWork : PanacheSoftwareUnitOfWork, IUnitOfWork
    {
        public UnitOfWork(PanacheSoftwareServiceTaskContext context, IUserProvider userProvider, IAPIHelper apiHelper) : base(context, userProvider)
        {
            TaskGroupHeaders = new TaskGroupHeaderRepository((PanacheSoftwareServiceTaskContext)_context, userProvider, apiHelper);
            TaskGroupDetails = new TaskGroupDetailRepository((PanacheSoftwareServiceTaskContext)_context, TaskGroupHeaders);
            TaskHeaders = new TaskHeaderRepository((PanacheSoftwareServiceTaskContext)_context, TaskGroupHeaders);
            TaskDetails = new TaskDetailRepository((PanacheSoftwareServiceTaskContext)_context);

            TemplateHeaders = new TemplateHeaderRepository((PanacheSoftwareServiceTaskContext)_context);
            TemplateDetails = new TemplateDetailRepository((PanacheSoftwareServiceTaskContext)_context);
            TemplateGroupHeaders = new TemplateGroupHeaderRepository((PanacheSoftwareServiceTaskContext)_context);
            TemplateGroupDetails = new TemplateGroupDetailRepository((PanacheSoftwareServiceTaskContext)_context);
            TemplateItemHeaders = new TemplateItemHeaderRepository((PanacheSoftwareServiceTaskContext)_context);
            TemplateItemDetails = new TemplateItemDetailRepository((PanacheSoftwareServiceTaskContext)_context);
        }

        public ITaskGroupHeaderRepository TaskGroupHeaders { get; private set; }
        public ITaskGroupDetailRepository TaskGroupDetails { get; private set; }
        public ITaskHeaderRepository TaskHeaders { get; private set; }
        public ITaskDetailRepository TaskDetails { get; private set; }

        public ITemplateHeaderRepository TemplateHeaders { get; private set; }
        public ITemplateDetailRepository TemplateDetails { get; private set; }
        public ITemplateGroupHeaderRepository TemplateGroupHeaders { get; private set; }
        public ITemplateGroupDetailRepository TemplateGroupDetails { get; private set; }
        public ITemplateItemHeaderRepository TemplateItemHeaders { get; private set; }
        public ITemplateItemDetailRepository TemplateItemDetails { get; private set; }
    }
}
