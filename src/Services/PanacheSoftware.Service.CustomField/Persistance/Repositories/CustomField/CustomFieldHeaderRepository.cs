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
    public class CustomFieldHeaderRepository : PanacheSoftwareRepository<CustomFieldHeader>, ICustomFieldHeaderRepository
    {

        public CustomFieldHeaderRepository(PanacheSoftwareServiceCustomFieldContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceCustomFieldContext PanacheSoftwareServiceCustomFieldContext
        {
            get { return Context as PanacheSoftwareServiceCustomFieldContext; }
        }

        public CustomFieldHeader GetCustomFieldHeader(Guid customFieldHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceCustomFieldContext.CustomFieldHeaders.AsNoTracking().SingleOrDefault(c => c.Id == customFieldHeaderId && c.Status != StatusTypes.Closed);

            return PanacheSoftwareServiceCustomFieldContext.CustomFieldHeaders.SingleOrDefault(c => c.Id == customFieldHeaderId && c.Status != StatusTypes.Closed);
        }
    }
}
