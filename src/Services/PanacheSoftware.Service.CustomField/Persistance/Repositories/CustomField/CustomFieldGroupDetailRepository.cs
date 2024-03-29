﻿using Microsoft.EntityFrameworkCore;
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
    public class CustomFieldGroupDetailRepository : PanacheSoftwareRepository<CustomFieldGroupDetail>, ICustomFieldGroupDetailRepository
    {

        public CustomFieldGroupDetailRepository(PanacheSoftwareServiceCustomFieldContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceCustomFieldContext PanacheSoftwareServiceCustomFieldContext
        {
            get { return Context as PanacheSoftwareServiceCustomFieldContext; }
        }

        public CustomFieldGroupDetail GetCustomFieldGroupDetail(Guid customFieldGroupDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupDetails.AsNoTracking().SingleOrDefault(c => c.Id == customFieldGroupDetailId && c.Status != StatusTypes.Closed);

            return PanacheSoftwareServiceCustomFieldContext.CustomFieldGroupDetails.SingleOrDefault(c => c.Id == customFieldGroupDetailId && c.Status != StatusTypes.Closed);
        }
    }
}
