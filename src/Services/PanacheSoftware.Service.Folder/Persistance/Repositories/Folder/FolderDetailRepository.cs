using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Database.Repositories;
using PanacheSoftware.Service.Folder.Core.Repositories;
using PanacheSoftware.Service.Folder.Persistance.Context;
using System;
using System.Linq;

namespace PanacheSoftware.Service.Folder.Persistance.Repositories.Folder
{
    public class FolderDetailRepository : PanacheSoftwareRepository<FolderDetail>, IFolderDetailRepository
    {
        private readonly IFolderHeaderRepository _folderHeaderRepository;

        public FolderDetailRepository(PanacheSoftwareServiceFolderContext context, IFolderHeaderRepository folderHeaderRepository) : base(context)
        {
            _folderHeaderRepository = folderHeaderRepository;
        }

        public PanacheSoftwareServiceFolderContext PanacheSoftwareServiceFolderContext
        {
            get { return Context as PanacheSoftwareServiceFolderContext; }
        }

        public FolderDetail GetFolderDetail(string folderShortName, bool readOnly)
        {
            return GetFolderDetail(_folderHeaderRepository.FolderNameToId(folderShortName), readOnly);
        }

        public FolderDetail GetFolderDetail(Guid folderHeaderId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderDetails.AsNoTracking().FirstOrDefault(t => t.FolderHeaderId == folderHeaderId);

            return PanacheSoftwareServiceFolderContext.FolderDetails.FirstOrDefault(t => t.FolderHeaderId == folderHeaderId);
        }

        public FolderDetail GetDetail(Guid folderDetailId, bool readOnly)
        {
            if (readOnly)
                return PanacheSoftwareServiceFolderContext.FolderDetails.AsNoTracking().SingleOrDefault(a => a.Id == folderDetailId);

            return PanacheSoftwareServiceFolderContext.FolderDetails.SingleOrDefault(a => a.Id == folderDetailId);
        }
    }
}
