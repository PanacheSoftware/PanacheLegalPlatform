﻿using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Core.Repositories
{
    public interface ICustomFieldHeaderRepository : IPanacheSoftwareRepository<CustomFieldHeader>
    {
        CustomFieldHeader GetCustomFieldHeader(Guid customFieldHeaderId, bool readOnly);
    }
}
