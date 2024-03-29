﻿using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Core.Repositories
{
    public interface IFileDetailRepository : IPanacheSoftwareRepository<FileDetail>
    {
        FileDetail GetFileDetailWithRelations(Guid fileHeaderId, bool readOnly);
        FileDetail GetDetailWithRelations(Guid fileDetailId, bool readOnly);
        Task<IEnumerable<FileDetail>> GetFileDetailsWithRelationsAsync(bool readOnly);
    }
}
