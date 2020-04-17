using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Database.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Folder.Core.Repositories
{
    public interface IFolderDetailRepository : IPanacheSoftwareRepository<FolderDetail>
    {
        FolderDetail GetFolderDetail(string folderShortName, bool readOnly);
        FolderDetail GetFolderDetail(Guid folderHeaderId, bool readOnly);
        FolderDetail GetDetail(Guid folderDetailId, bool readOnly);
    }
}
