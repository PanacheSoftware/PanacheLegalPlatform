using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.Folder;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Service.Folder.Manager
{
    public interface IFolderManager
    {
        FolderList GetFolderList(Guid folderHeaderId = new Guid(), bool validParents = false);
        FolderList GetMainFolders();
        List<Guid> GetChildFolderIds(Guid folderHeaderId);
        FolderStruct GetFolderStructure(Guid folderHeaderId);
        //FolderList GetFoldersForUser(Guid userDetailId);
        bool FolderParentOkay(FolderHeader folderHeader);
        bool SetNewFolderSequenceNo(FolderHeader folderHeader);
        bool SetNewFolderNodeSequenceNo(FolderNode folderNode);
    }
}
