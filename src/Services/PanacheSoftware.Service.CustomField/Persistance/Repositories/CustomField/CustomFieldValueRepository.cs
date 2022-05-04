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
    public class CustomFieldValueRepository : PanacheSoftwareRepository<CustomFieldValue>, ICustomFieldValueRepository
    {
        public CustomFieldValueRepository(PanacheSoftwareServiceCustomFieldContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceCustomFieldContext PanacheSoftwareServiceCustomFieldContext
        {
            get { return Context as PanacheSoftwareServiceCustomFieldContext; }
        }

        public async Task<IEnumerable<CustomFieldValue>> GetCustomFieldValuesForLinkAsync(Guid linkId, string linkType, bool readOnly)
        {

            if(readOnly)
                return await PanacheSoftwareServiceCustomFieldContext.CustomFieldValues
                    .Include(v => v.CustomFieldValueHistorys)
                    .Include(h => h.CustomFieldHeader)
                    .AsNoTracking()
                    .Where(v => v.LinkId == linkId).ToListAsync();

            return await PanacheSoftwareServiceCustomFieldContext.CustomFieldValues
                .Include(v => v.CustomFieldValueHistorys)
                .Include(h => h.CustomFieldHeader)
                .Where(v => v.LinkId == linkId).ToListAsync();
        }

        public CustomFieldValue GetCustomFieldValue(Guid linkId, string linkType, Guid customFieldHeaderId, bool readOnly)
        {
            if(readOnly)
                return PanacheSoftwareServiceCustomFieldContext.CustomFieldValues
                    .AsNoTracking()
                    .FirstOrDefault(v => v.LinkId == linkId && v.LinkType == linkType && v.CustomFieldHeaderId == customFieldHeaderId);

            return PanacheSoftwareServiceCustomFieldContext.CustomFieldValues
                    .FirstOrDefault(v => v.LinkId == linkId && v.LinkType == linkType && v.CustomFieldHeaderId == customFieldHeaderId);
        }
    }
}
