using PanacheSoftware.Database.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Core.Repositories
{
    public interface IUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        IFileHeaderRepository FileHeaders { get; }
        IFileDetailRepository FileDetails { get; }
        IFileVersionRepository FileVersions { get; }
        IFileLinkRepository FileLinks { get; }
    }
}
