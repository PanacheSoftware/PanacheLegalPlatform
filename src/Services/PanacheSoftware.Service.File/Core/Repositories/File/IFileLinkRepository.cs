using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Core.Repositories
{
    public interface IFileLinkRepository : IPanacheSoftwareRepository<FileLink>
    {
        Task<IEnumerable<FileLink>> GetFileLinksWithRelationsAsync(Guid fileHeaderId, bool readOnly);
        Task<FileLink> GetLinkWithRelationsAsync(Guid fileLinkId, bool readOnly);
        Task<IEnumerable<FileLink>> GetLinksWithRelationsAsync(bool readOnly);
        Task<IEnumerable<FileLink>> GetFileLinksWithRelationsForLinkAsync(Guid linkId, string linkType, bool readOnly);
    }
}
