using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Database.Persistance;
using PanacheSoftware.Http;
using PanacheSoftware.Service.Folder.Core;
using PanacheSoftware.Service.Folder.Core.Repositories;
using PanacheSoftware.Service.Folder.Persistance.Context;
using PanacheSoftware.Service.Folder.Persistance.Repositories.Folder;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Folder.Persistance
{
    public class UnitOfWork : PanacheSoftwareUnitOfWork, IUnitOfWork
    {
        public UnitOfWork(PanacheSoftwareServiceFolderContext context, IUserProvider userProvider) : base(context, userProvider)
        {
            FolderHeaders = new FolderHeaderRepository((PanacheSoftwareServiceFolderContext)_context);
            FolderDetails = new FolderDetailRepository((PanacheSoftwareServiceFolderContext)_context, FolderHeaders);
            FolderNodes = new FolderNodeRepository((PanacheSoftwareServiceFolderContext)_context, FolderHeaders);
            FolderNodeDetails = new FolderNodeDetailRepository((PanacheSoftwareServiceFolderContext)_context);
        }

        public IFolderHeaderRepository FolderHeaders { get; private set; }
        public IFolderDetailRepository FolderDetails { get; private set; }
        public IFolderNodeRepository FolderNodes { get; private set; }
        public IFolderNodeDetailRepository FolderNodeDetails { get; private set; }
    }
}
