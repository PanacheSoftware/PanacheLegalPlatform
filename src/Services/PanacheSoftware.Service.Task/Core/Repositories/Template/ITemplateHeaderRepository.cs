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
        Task<TemplateHeader> GetTemplateHeaderWithRelationsAsync(string templateHeaderShortName, bool readOnly, string accessToken);
        Task<TemplateHeader> GetTemplateHeaderWithRelationsAsync(Guid templateHeaderId, bool readOnly, string accessToken);
    }
}
