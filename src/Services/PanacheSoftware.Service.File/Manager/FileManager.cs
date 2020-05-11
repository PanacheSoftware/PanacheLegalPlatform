using AutoMapper;
using PanacheSoftware.Core.Domain.API.File;
using PanacheSoftware.Core.Domain.File;
using PanacheSoftware.Service.File.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.Service.File.Manager
{
    public class FileManager : IFileManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FileManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FileList> GetFileListForLinkAsync(Guid linkId, string linkType)
        {
            var fileList = new FileList();

            var fileLinks = await _unitOfWork.FileLinks.GetFileLinksWithRelationsForLinkAsync(linkId, linkType, true);

            if(fileLinks.Any())
            {
                foreach (FileLink fileLink in fileLinks)
                {
                    var fileHeader = await _unitOfWork.FileHeaders.GetFileHeaderWithRelationsAsync(fileLink.FileHeaderId, true);

                    if (fileHeader != null)
                        fileList.FileHeaders.Add(_mapper.Map<FileHead>(fileHeader));
                }
            }

            return fileList;
        }

        public async Task<bool> SetFileVersionSequenceNoAsync(FileVersion fileVersion)
        {
            var fileHeader = await _unitOfWork.FileHeaders.GetFileHeaderWithRelationsAsync(fileVersion.FileHeaderId, true);

            if(fileHeader != null)
            {
                var maxFileVersion = fileHeader.FileVersions.OrderByDescending(f => f.VersionNumber).FirstOrDefault();

                if(maxFileVersion != null)
                {
                    fileVersion.VersionNumber = maxFileVersion.VersionNumber + 1;
                }
            }

            return true;
        }
    }
}
