using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Core.Repositories
{
    public interface ICustomFieldGroupDetailRepository : IPanacheSoftwareRepository<CustomFieldGroupDetail>
    {
        CustomFieldGroupDetail GetCustomFieldGroupDetail(Guid customFieldGroupDetailId, bool readOnly);
    }
}
