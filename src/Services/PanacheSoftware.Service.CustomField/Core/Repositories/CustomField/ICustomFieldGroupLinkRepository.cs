using PanacheSoftware.Core.Domain.CustomField;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.CustomField.Core.Repositories
{
    public interface ICustomFieldGroupLinkRepository : IPanacheSoftwareRepository<CustomFieldGroupLink>
    {
        CustomFieldGroupLink GetCustomFieldGroupLink(Guid customFieldGroupLinkId, bool readOnly);
        Task<IEnumerable<CustomFieldGroupLink>> GetCustomFieldGroupLinksWithRelationsForLinkAsync(Guid linkId, string linkType, bool readOnly);

    }
}
