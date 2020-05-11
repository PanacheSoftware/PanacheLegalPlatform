using PanacheSoftware.Database.Persistance;
using PanacheSoftware.Http;
using PanacheSoftware.Service.File.Core.Repositories;
using PanacheSoftware.Service.File.Persistance.Context;
using PanacheSoftware.Service.File.Persistance.Repositories.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Persistance
{
    public class UnitOfWork : PanacheSoftwareUnitOfWork, IUnitOfWork
    {
        public UnitOfWork(PanacheSoftwareServiceFileContext context, IUserProvider userProvider) : base(context, userProvider)
        {
            FileHeaders = new FileHeaderRepository(context);
            FileDetails = new FileDetailRepository(context);
            FileVersions = new FileVersionRepository(context);
            FileLinks = new FileLinkRepository(context);
        }

        public IFileHeaderRepository FileHeaders { get; private set; }

        public IFileDetailRepository FileDetails { get; private set; }

        public IFileVersionRepository FileVersions { get; private set; }

        public IFileLinkRepository FileLinks { get; private set; }
    }
}
