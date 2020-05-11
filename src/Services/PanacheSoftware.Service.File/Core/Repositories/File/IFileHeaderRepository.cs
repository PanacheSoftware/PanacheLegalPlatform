using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Core.Repositories
{
    public interface IFileHeaderRepository : IPanacheSoftwareRepository<FileHeader>
    {
        Task<IEnumerable<FileHeader>> GetFileHeadersWithRelationsAsync(bool readOnly);
        Task<FileHeader> GetFileHeaderWithRelationsAsync(Guid fileHeaderId, bool readOnly);
    }
}
