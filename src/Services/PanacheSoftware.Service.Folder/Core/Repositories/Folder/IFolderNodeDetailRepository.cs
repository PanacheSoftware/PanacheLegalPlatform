using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Database.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Folder.Core.Repositories
{
    public interface IFolderNodeDetailRepository : IPanacheSoftwareRepository<FolderNodeDetail>
    {
        FolderNodeDetail GetFolderNodeDetail(Guid folderNodeId, bool readOnly);
        FolderNodeDetail GetNodeDetail(Guid folderNodeDetailId, bool readOnly);
    }
}
