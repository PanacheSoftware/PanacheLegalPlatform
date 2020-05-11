using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.File.Core.Repositories;
using PanacheSoftware.Service.File.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Persistance.Repositories.File
{
    public class FileHeaderRepository : PanacheSoftwareRepository<FileHeader>, IFileHeaderRepository
    {

        public FileHeaderRepository(PanacheSoftwareServiceFileContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFileContext PanacheSoftwareServiceFileContext
        {
            get { return Context as PanacheSoftwareServiceFileContext; }
        }

        public async Task<FileHeader> GetFileHeaderWithRelationsAsync(Guid fileHeaderId, bool readOnly)
        {

            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileHeaders
                    .Include(h => h.FileDetail)
                    .Include(h => h.FileLinks)
                    .Include(h => h.FileVersions)
                    .AsNoTracking().SingleOrDefaultAsync(h => h.Id == fileHeaderId);

            return await PanacheSoftwareServiceFileContext.FileHeaders
                    .Include(h => h.FileDetail)
                    .Include(h => h.FileLinks)
                    .Include(h => h.FileVersions)
                    .SingleOrDefaultAsync(h => h.Id == fileHeaderId);
        }

        public async Task<IEnumerable<FileHeader>> GetFileHeadersWithRelationsAsync(bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileHeaders
                    .Include(h => h.FileDetail)
                    .Include(h => h.FileLinks)
                    .Include(h => h.FileVersions)
                    .AsNoTracking().ToListAsync();

            return await PanacheSoftwareServiceFileContext.FileHeaders
                    .Include(h => h.FileDetail)
                    .Include(h => h.FileLinks)
                    .Include(h => h.FileVersions)
                    .ToListAsync();
        }
    }
}
