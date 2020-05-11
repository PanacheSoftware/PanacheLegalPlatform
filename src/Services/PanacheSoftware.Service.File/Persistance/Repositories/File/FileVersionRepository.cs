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
    public class FileVersionRepository : PanacheSoftwareRepository<FileVersion>, IFileVersionRepository
    {
        public FileVersionRepository(PanacheSoftwareServiceFileContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFileContext PanacheSoftwareServiceFileContext
        {
            get { return Context as PanacheSoftwareServiceFileContext; }
        }

        public async Task<IEnumerable<FileVersion>> GetFileVersionsWithRelationsAsync(Guid fileHeaderId, bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileVersions
                    .Include(v => v.FileHeader)
                    .AsNoTracking()
                    .Where(v => v.FileHeaderId == fileHeaderId).ToListAsync();

            return await PanacheSoftwareServiceFileContext.FileVersions
                    .Include(v => v.FileHeader)
                    .Where(v => v.FileHeaderId == fileHeaderId).ToListAsync();
        }

        public async Task<IEnumerable<FileVersion>> GetVersionsWithRelationsAsync(bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileVersions
                    .Include(v => v.FileHeader)
                    .AsNoTracking().ToListAsync();

            return await PanacheSoftwareServiceFileContext.FileVersions
                    .Include(v => v.FileHeader).ToListAsync();
        }

        public async Task<FileVersion> GetVersionWithRelationsAsync(Guid fileVersionId, bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileVersions
                    .Include(v => v.FileHeader)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == fileVersionId);

            return await PanacheSoftwareServiceFileContext.FileVersions
                    .Include(v => v.FileHeader)
                    .FirstOrDefaultAsync(v => v.Id == fileVersionId);
        }
    }
}
