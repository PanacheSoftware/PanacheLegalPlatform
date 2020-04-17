using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Folder.Core.Repositories;
using PanacheSoftware.Service.Folder.Persistance.Context;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Folder.Persistance.Repositories.Folder
{
    public class FolderNodeDetailRepository : PanacheSoftwareRepository<FolderNodeDetail>, IFolderNodeDetailRepository
    {
        public FolderNodeDetailRepository(PanacheSoftwareServiceFolderContext context) : base(context)
        {
        }

        public PanacheSoftwareServiceFolderContext PanacheSoftwareServiceFolderContext
        {
            get { return Context as PanacheSoftwareServiceFolderContext; }
        }

        public FolderNodeDetail GetFolderNodeDetail(Guid folderNodeId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderNodeDetails.AsNoTracking().FirstOrDefault(fnd => fnd.FolderNodeId == folderNodeId);

            return PanacheSoftwareServiceFolderContext.FolderNodeDetails.FirstOrDefault(fnd => fnd.FolderNodeId == folderNodeId);
        }

        public FolderNodeDetail GetNodeDetail(Guid folderNodeDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderNodeDetails.AsNoTracking().FirstOrDefault(fnd => fnd.Id == folderNodeDetailId);

            return PanacheSoftwareServiceFolderContext.FolderNodeDetails.FirstOrDefault(fnd => fnd.Id == folderNodeDetailId);
        }
    }
}
