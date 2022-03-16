using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Task.Template;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Task.Core.Repositories;
using PanacheSoftware.Service.Task.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.Task.Persistance.Repositories.Template
{
    public class TemplateHeaderRepository : PanacheSoftwareRepository<TemplateHeader>, ITemplateHeaderRepository
    {
        private readonly IUserProvider _userProvider;
        private readonly IAPIHelper _apiHelper;
        //private List<Guid> _userTeams;

        public TemplateHeaderRepository(PanacheSoftwareServiceTaskContext context, IUserProvider userProvider, IAPIHelper apiHelper) : base(context)
        {
            _userProvider = userProvider;
            _apiHelper = apiHelper;
        }

        public PanacheSoftwareServiceTaskContext PanacheSoftwareServiceTaskContext
        {
            get { return Context as PanacheSoftwareServiceTaskContext; }
        }

        public TemplateHeader GetTemplateHeaderWithRelations(string templateHeaderShortName, bool readOnly, string accessToken)
        {
            return GetTemplateHeaderWithRelations(TemplateHeaderNameToId(templateHeaderShortName), readOnly, accessToken);
        }

        public TemplateHeader GetTemplateHeaderWithRelations(Guid templateHeaderId, bool readOnly, string accessToken)
        {
            if (readOnly)
                return PanacheSoftwareServiceTaskContext.TemplateHeaders
                .Include(t => t.TemplateDetail)
                .Include(t => t.TemplateGroupHeaders.Where(ct => ct.Status != StatusTypes.Closed))
                    .ThenInclude(i => i.TemplateItemHeaders.Where(ct => ct.Status != StatusTypes.Closed))
                    .ThenInclude(id => id.TemplateItemDetail)
                .Include(t => t.TemplateGroupHeaders.Where(ct => ct.Status != StatusTypes.Closed))
                    .ThenInclude(d => d.TemplateGroupDetail)
                .AsNoTracking()
                .SingleOrDefault(t => t.Id == templateHeaderId && t.Status != StatusTypes.Closed);

            return PanacheSoftwareServiceTaskContext.TemplateHeaders
                .Include(t => t.TemplateDetail)
                .Include(t => t.TemplateGroupHeaders.Where(ct => ct.Status != StatusTypes.Closed))
                    .ThenInclude(i => i.TemplateItemHeaders.Where(ct => ct.Status != StatusTypes.Closed))
                        .ThenInclude(id => id.TemplateItemDetail)
                .Include(t => t.TemplateGroupHeaders.Where(ct => ct.Status != StatusTypes.Closed))
                    .ThenInclude(d => d.TemplateGroupDetail)
                .SingleOrDefault(t => t.Id == templateHeaderId && t.Status != StatusTypes.Closed);
        }

        public Guid TemplateHeaderNameToId(string templateHeaderShortName)
        {
            Guid foundGuid = Guid.Empty;

            var foundTemplateHeader =
                PanacheSoftwareServiceTaskContext.TemplateHeaders.AsNoTracking().SingleOrDefault(t => t.ShortName == templateHeaderShortName);

            if (foundTemplateHeader != null)
                foundGuid = foundTemplateHeader.Id;

            return foundGuid;
        }
    }
}
