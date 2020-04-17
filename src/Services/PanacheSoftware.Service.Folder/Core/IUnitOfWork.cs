using PanacheSoftware.Database.Core;
using PanacheSoftware.Service.Folder.Core.Repositories;
using System;

namespace PanacheSoftware.Service.Folder.Core
{
    public interface IUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        IFolderHeaderRepository FolderHeaders { get; }
        IFolderDetailRepository FolderDetails { get; }
        IFolderNodeRepository FolderNodes { get; }
        IFolderNodeDetailRepository FolderNodeDetails { get; }
    }
}
