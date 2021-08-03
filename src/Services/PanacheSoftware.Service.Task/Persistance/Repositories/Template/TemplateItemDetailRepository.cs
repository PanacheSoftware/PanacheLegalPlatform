using PanacheSoftware.Core.Domain.Task.Template;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Task.Core.Repositories;
using PanacheSoftware.Service.Task.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.Repositories.Template
{
    public class TemplateItemDetailRepository : PanacheSoftwareRepository<TemplateItemDetail>, ITemplateItemDetailRepository
    {
        public TemplateItemDetailRepository(PanacheSoftwareServiceTaskContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceTaskContext PanacheSoftwareServiceTaskContext
        {
            get { return Context as PanacheSoftwareServiceTaskContext; }
        }
    }
}
