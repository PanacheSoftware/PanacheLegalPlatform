using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Core.Repositories
{
    public interface IFileVersionRepository : IPanacheSoftwareRepository<FileVersion>
    {
        Task<IEnumerable<FileVersion>> GetFileVersionsWithRelationsAsync(Guid fileHeaderId, bool readOnly);
        Task<FileVersion> GetVersionWithRelationsAsync(Guid fileVersionId, bool readOnly);
        Task<IEnumerable<FileVersion>> GetVersionsWithRelationsAsync(bool readOnly);
    }
}
