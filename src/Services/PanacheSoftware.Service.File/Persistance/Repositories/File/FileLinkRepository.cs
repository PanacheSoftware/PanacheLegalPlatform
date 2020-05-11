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
    public class FileLinkRepository : PanacheSoftwareRepository<FileLink>, IFileLinkRepository
    {
        public FileLinkRepository(PanacheSoftwareServiceFileContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFileContext PanacheSoftwareServiceFileContext
        {
            get { return Context as PanacheSoftwareServiceFileContext; }
        }

        public async Task<IEnumerable<FileLink>> GetFileLinksWithRelationsForLinkAsync(Guid linkId, string linkType, bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileLinks
                    //.Include(l => l.FileHeader)
                    //    .ThenInclude(h => h.FileDetail)
                    //.Include(l => l.FileHeader)
                    //    .ThenInclude(h => h.FileVersions)
                    .AsNoTracking()
                    .Where(l => l.LinkId == linkId && l.LinkType == linkType).ToListAsync();

            return await PanacheSoftwareServiceFileContext.FileLinks
                //.Include(l => l.FileHeader)
                //    .ThenInclude(h => h.FileDetail)
                //.Include(l => l.FileHeader)
                //    .ThenInclude(h => h.FileVersions)
                .Where(l => l.LinkId == linkId && l.LinkType == linkType).ToListAsync();
        }

        public async Task<IEnumerable<FileLink>> GetFileLinksWithRelationsAsync(Guid fileHeaderId, bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileLinks
                    .Include(l => l.FileHeader)
                    .AsNoTracking()
                    .Where(l => l.FileHeaderId == fileHeaderId).ToListAsync();

            return await PanacheSoftwareServiceFileContext.FileLinks
                    .Include(l => l.FileHeader)
                    .Where(l => l.FileHeaderId == fileHeaderId).ToListAsync();
        }

        public async Task<IEnumerable<FileLink>> GetLinksWithRelationsAsync(bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileLinks
                    .Include(l => l.FileHeader)
                    .AsNoTracking()
                    .ToListAsync();

            return await PanacheSoftwareServiceFileContext.FileLinks
                    .Include(l => l.FileHeader)
                    .ToListAsync();
        }

        public async Task<FileLink> GetLinkWithRelationsAsync(Guid fileLinkId, bool readOnly)
        {
            if (readOnly)
                return await PanacheSoftwareServiceFileContext.FileLinks
                    .Include(l => l.FileHeader)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Id == fileLinkId);

            return await PanacheSoftwareServiceFileContext.FileLinks
                    .Include(l => l.FileHeader)
                    .FirstOrDefaultAsync(l => l.Id == fileLinkId);
        }
    }
}
