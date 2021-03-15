using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Core.Types;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.CustomField.Core.Repositories;
using PanacheSoftware.Service.CustomField.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.Repositories.CustomField
{
    public class CustomFieldGroupLinkRepository : PanacheSoftwareRepository<CustomFieldGroupLink>, ICustomFieldGroupLinkRepository
    {

        public CustomFieldGroupLinkRepository(PanacheSoftwareServiceCustomFieldContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceCustomFieldContext PanacheSoftwareServiceCustomFieldContext
        {
            get { return Context as PanacheSoftwareServiceCustomFieldContext; }
        }

        public CustomFieldGroupLink GetCustomFieldGroupLink(Guid customFieldGroupLinkId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupLinks.AsNoTracking().SingleOrDefault(c => c.Id == customFieldGroupLinkId && c.Status != StatusTypes.Closed);

            return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupLinks.SingleOrDefault(c => c.Id == customFieldGroupLinkId && c.Status != StatusTypes.Closed);
        }

        public async Task<IEnumerable<CustomFieldGroupLink>> GetFileLinksWithRelationsForLinkAsync(Guid linkId, string linkType, bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupLinks
                    .Include(l => l.CustomFieldGroupHeader)
                    .ThenInclude(cf => cf.CustomFieldHeaders)
                    .AsNoTracking()
                    .Where(l => l.LinkId == linkId && l.LinkType == linkType).ToListAsync();

            return await PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupLinks
                    .Include(l => l.CustomFieldGroupHeader)
                    .ThenInclude(cf => cf.CustomFieldHeaders)
                    .Where(l => l.LinkId == linkId && l.LinkType == linkType).ToListAsync();
        }
    }
}
