using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Folder.Core.Repositories;
using PanacheSoftware.Service.Folder.Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PanacheSoftware.Service.Folder.Persistance.Repositories.Folder
{
    public class FolderHeaderRepository : PanacheSoftwareRepository<FolderHeader>, IFolderHeaderRepository
    {
        public FolderHeaderRepository(PanacheSoftwareServiceFolderContext context) : base(context)
        {

        }

        public PanacheSoftwareServiceFolderContext PanacheSoftwareServiceFolderContext
        {
            get { return Context as PanacheSoftwareServiceFolderContext; }
        }

        public FolderHeader GetFolderHeader(string folderShortName, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderHeaders.AsNoTracking().SingleOrDefault(c => c.Id == FolderNameToId(folderShortName));

            return PanacheSoftwareServiceFolderContext.FolderHeaders.Find(FolderNameToId(folderShortName));
        }

        public FolderHeader GetFolderHeaderWithRelations(string folderShortName, bool readOnly)
        {
            return GetFolderHeaderWithRelations(FolderNameToId(folderShortName), readOnly);
        }

        public FolderHeader GetFolderHeaderWithRelations(Guid folderHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderHeaders
                .Include(t => t.FolderDetail)
                .Include(t => t.ChildFolders)
                .Include(t => t.ParentFolder)
                .Include(t => t.ChildNodes)
                .AsNoTracking()
                .SingleOrDefault(t => t.Id == folderHeaderId);

            return PanacheSoftwareServiceFolderContext.FolderHeaders
                .Include(t => t.FolderDetail)
                .Include(t => t.ChildFolders)
                .Include(t => t.ParentFolder)
                .Include(t => t.ChildNodes)
                .SingleOrDefault(t => t.Id == folderHeaderId);
        }

        public List<FolderHeader> GetFolderTree(string folderShortName)
        {
            return GetFolderTree(FolderNameToId(folderShortName));
        }

        public List<FolderHeader> GetFolderTree(Guid folderHeaderId)
        {
            return PanacheSoftwareServiceFolderContext.FolderHeaders
                .Include(t => t.FolderDetail)
                .Include(t => t.ChildFolders)
                .Include(t => t.ChildNodes)
                .AsEnumerable()
                .Where(t => t.Id == folderHeaderId).ToList();
        }

        /// <summary>
        /// Returns the TeamHeader ID corresponding to a ShortName
        /// </summary>
        /// <param name="teamShortName">TeamHeader ShortName</param>
        /// <returns>A valid TeamHeader ID or Guid.Empty if no TeamHeader found</returns>
        public Guid FolderNameToId(string folderShortName)
        {
            Guid foundGuid = Guid.Empty;

            FolderHeader foundFolderHeader =
                PanacheSoftwareServiceFolderContext.FolderHeaders.AsNoTracking().SingleOrDefault(t => t.ShortName == folderShortName);

            if (foundFolderHeader != null)
                foundGuid = foundFolderHeader.Id;

            return foundGuid;
        }

        public List<FolderHeader> GetMainFolders()
        {
            return PanacheSoftwareServiceFolderContext.FolderHeaders.AsEnumerable().Where(f => f.ParentFolderId == null).ToList();
        }
    }
}
