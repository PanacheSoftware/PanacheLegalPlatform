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
    public class FolderNodeRepository : PanacheSoftwareRepository<FolderNode>, IFolderNodeRepository
    {
        private readonly IFolderHeaderRepository _folderHeaderRepository;

        public FolderNodeRepository(PanacheSoftwareServiceFolderContext context, IFolderHeaderRepository folderHeaderRepository) : base(context)
        {
            _folderHeaderRepository = folderHeaderRepository;
        }

        public PanacheSoftwareServiceFolderContext PanacheSoftwareServiceFolderContext
        {
            get { return Context as PanacheSoftwareServiceFolderContext; }
        }

        public IEnumerable<FolderNode> GetFolderNodes(string folderShortName, bool readOnly)
        {
            return GetFolderNodes(_folderHeaderRepository.FolderNameToId(folderShortName), readOnly);
        }

        public IEnumerable<FolderNode> GetFolderNodes(Guid folderHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderNodes.Include(n => n.FolderNodeDetail).AsNoTracking().Where(f => f.FolderHeaderId == folderHeaderId);

            return PanacheSoftwareServiceFolderContext.FolderNodes.Include(n => n.FolderNodeDetail).Where(f => f.FolderHeaderId == folderHeaderId);
        }

        public FolderNode GetNode(Guid folderNodeId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderNodes.Include(n => n.FolderNodeDetail).AsNoTracking().SingleOrDefault(a => a.Id == folderNodeId);

            return PanacheSoftwareServiceFolderContext.FolderNodes.Include(n => n.FolderNodeDetail).SingleOrDefault(a => a.Id == folderNodeId);
        }
    }
}