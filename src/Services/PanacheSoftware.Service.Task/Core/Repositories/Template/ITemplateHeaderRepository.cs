﻿using PanacheSoftware.Core.Domain.Task.Template;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Core.Repositories
{
    public interface ITemplateHeaderRepository : IPanacheSoftwareRepository<TemplateHeader>
    {
        TemplateHeader GetTemplateHeaderWithRelations(string templateHeaderShortName, bool readOnly, string accessToken);
        TemplateHeader GetTemplateHeaderWithRelations(Guid templateHeaderId, bool readOnly, string accessToken);
        IList<TemplateHeader> GetTemplateHeadersWithDetails(bool readOnly);
    }
}
