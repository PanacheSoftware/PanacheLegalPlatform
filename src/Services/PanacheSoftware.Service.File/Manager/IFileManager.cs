using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Manager
{
    public interface IFileManager
    {
        Task<FileList> GetFileListForLinkAsync(Guid linkId, string linkType);
        Task<bool> SetFileVersionSequenceNoAsync(FileVersion fileVersion);
    }
}
