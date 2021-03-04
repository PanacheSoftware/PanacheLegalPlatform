using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.CustomField.Core.Repositories;
using PanacheSoftware.Service.CustomField.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Persistance.Repositories.CustomField
{
    public class CustomFieldGroupHeaderRepository : PanacheSoftwareRepository<CustomFieldGroupHeader>, ICustomFieldGroupHeaderRepository
    {

        public CustomFieldGroupHeaderRepository(PanacheSoftwareServiceCustomFieldContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceCustomFieldContext PanacheSoftwareServiceCustomFieldContext
        {
            get { return Context as PanacheSoftwareServiceCustomFieldContext; }
        }

        public CustomFieldGroupHeader GetCustomFieldGroupHeader(Guid customFieldGroupHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupHeaders.AsNoTracking().SingleOrDefault(c => c.Id == customFieldGroupHeaderId);

            return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupHeaders.SingleOrDefault(c => c.Id == customFieldGroupHeaderId);
        }

        public CustomFieldGroupHeader GetCustomFieldGroupHeaderWithRelations(Guid customFieldGroupHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupHeaders
                .Include(h => h.CustomFieldGroupDetail)
                .Include(h => h.CustomFieldHeaders)
                .AsNoTracking()
                .SingleOrDefault(c => c.Id == customFieldGroupHeaderId);

            return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupHeaders
                .Include(h => h.CustomFieldGroupDetail)
                .Include(h => h.CustomFieldHeaders)
                .SingleOrDefault(c => c.Id == customFieldGroupHeaderId);
        }
    }
}
