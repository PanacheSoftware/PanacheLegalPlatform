﻿using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Core.Repositories
{
    public interface ICustomFieldValueRepository : IPanacheSoftwareRepository<CustomFieldValue>
    {
        Task<IEnumerable<CustomFieldValue>> GetCustomFieldValuesForLinkAsync(Guid linkId, string linkType, bool readOnly);
        CustomFieldValue GetCustomFieldValue(Guid linkId, string linkType, Guid customFieldHeaderId, bool readOnly);
    }
}
