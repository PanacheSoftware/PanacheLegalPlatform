using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Folder.Core.Repositories
{
    public interface IFolderHeaderRepository : IPanacheSoftwareRepository<FolderHeader>
    {
        FolderHeader GetFolderHeader(string folderShortName, bool readOnly);
        FolderHeader GetFolderHeaderWithRelations(string folderShortName, bool readOnly);
        FolderHeader GetFolderHeaderWithRelations(Guid folderHeaderId, bool readOnly);
        Guid FolderNameToId(string folderShortName);
        List<FolderHeader> GetFolderTree(string folderShortName);
        List<FolderHeader> GetFolderTree(Guid folderHeaderId);
        List<FolderHeader> GetMainFolders();
    }
}
