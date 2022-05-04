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
    public class CustomFieldValueHistoryRepository : PanacheSoftwareRepository<CustomFieldValueHistory>, ICustomFieldValueHistoryRepository
    {

        public CustomFieldValueHistoryRepository(PanacheSoftwareServiceCustomFieldContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceCustomFieldContext PanacheSoftwareServiceCustomFieldContext
        {
            get { return Context as PanacheSoftwareServiceCustomFieldContext; }
        }

        public IList<CustomFieldValueHistory> GetCustomFieldValueHistorys(Guid customFieldValueId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceCustomFieldContext.CustomFieldValueHistorys
                .AsNoTracking()
                .Where(h => h.CustomFieldValueId == customFieldValueId).ToList();

            return PanacheSoftwareServiceCustomFieldContext.CustomFieldValueHistorys
                .Where(h => h.CustomFieldValueId == customFieldValueId).ToList();
        }
    }
}
