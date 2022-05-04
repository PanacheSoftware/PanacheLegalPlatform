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
    public class FileDetailRepository : PanacheSoftwareRepository<FileDetail>, IFileDetailRepository
    {
        public FileDetailRepository(PanacheSoftwareServiceFileContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFileContext PanacheSoftwareServiceFileContext
        {
            get { return Context as PanacheSoftwareServiceFileContext; }
        }

        public FileDetail GetDetailWithRelations(Guid fileDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFileContext.FileDetails
                    .Include(d => d.FileHeader)
                    .AsNoTracking()
                    .SingleOrDefault(d => d.Id == fileDetailId);

            return PanacheSoftwareServiceFileContext.FileDetails
                    .Include(d => d.FileHeader)
                    .SingleOrDefault(d => d.Id == fileDetailId);
        }

        public async Task<IEnumerable<FileDetail>> GetFileDetailsWithRelationsAsync(bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileDetails
                    .Include(d => d.FileHeader)
                    .AsNoTracking().ToListAsync();

            return await PanacheSoftwareServiceFileContext.FileDetails
                    .Include(d => d.FileHeader).ToListAsync();
        }

        public FileDetail GetFileDetailWithRelations(Guid fileHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFileContext.FileDetails
                    .Include(d => d.FileHeader)
                    .AsNoTracking()
                    .SingleOrDefault(d => d.FileHeaderId == fileHeaderId);

            return PanacheSoftwareServiceFileContext.FileDetails
                    .Include(d => d.FileHeader)
                    .AsNoTracking()
                    .SingleOrDefault(d => d.FileHeaderId == fileHeaderId);
        }
    }
}
