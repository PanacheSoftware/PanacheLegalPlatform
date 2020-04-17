using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Folder.Core.Repositories
{
    public interface IFolderNodeRepository : IPanacheSoftwareRepository<FolderNode>
    {
        IEnumerable<FolderNode> GetFolderNodes(string folderShortName, bool readOnly);
        IEnumerable<FolderNode> GetFolderNodes(Guid folderHeaderId, bool readOnly);
        FolderNode GetNode(Guid folderNodeId, bool readOnly);
    }
}
