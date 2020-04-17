using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PanacheSoftware.Core.Domain.API.Folder;
using PanacheSoftware.Core.Domain.Folder;
using PanacheSoftware.Service.Folder.Core;

namespace PanacheSoftware.Service.Folder.Manager
{
    public class FolderManager : IFolderManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FolderManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool FolderParentOkay(FolderHeader folderHeader)
        {
            Guid parentFolderId = folderHeader.ParentFolderId ?? Guid.Empty;
            if (parentFolderId != Guid.Empty)
            {
                FolderHeader parentFolderHeader = _unitOfWork.FolderHeaders.Get(parentFolderId);

                if (parentFolderHeader == null)
                {
                    return false;
                }

                folderHeader.ClientHeaderId = Guid.Empty;
                folderHeader.TeamHeaderId = Guid.Empty;
                folderHeader.MainUserId = Guid.Empty;
            }
            else
            {
                folderHeader.ParentFolderId = null;
            }

            return true;
        }

        public bool SetNewFolderSequenceNo(FolderHeader folderHeader)
        {
            folderHeader.SequenceNumber = 0;

            Guid parentFolderId = folderHeader.ParentFolderId ?? Guid.Empty;
            if (parentFolderId != Guid.Empty)
            {
                FolderHeader parentFolder = _unitOfWork.FolderHeaders.GetFolderHeaderWithRelations(parentFolderId, true);

                if(parentFolder != null)
                {
                    var maxChild = parentFolder.ChildFolders.OrderByDescending(f => f.SequenceNumber).FirstOrDefault();

                    if(maxChild != null)
                    {
                        folderHeader.SequenceNumber = maxChild.SequenceNumber + 1;
                    }
                }
            }

            return true;
        }

        public bool SetNewFolderNodeSequenceNo(FolderNode folderNode)
        {
            folderNode.SequenceNumber = 0;

            Guid parentFolderId = folderNode.FolderHeaderId;
            if (parentFolderId != Guid.Empty)
            {
                FolderHeader parentFolder = _unitOfWork.FolderHeaders.GetFolderHeaderWithRelations(parentFolderId, true);

                if (parentFolder != null)
                {
                    var maxChildNode = parentFolder.ChildNodes.OrderByDescending(f => f.SequenceNumber).FirstOrDefault();

                    if (maxChildNode != null)
                    {
                        folderNode.SequenceNumber = maxChildNode.SequenceNumber + 1;
                    }
                }
            }

            return true;
        }

        public List<Guid> GetChildFolderIds(Guid folderHeaderId)
        {
            List<Guid> childFolders = new List<Guid>();

            var folderTree = _unitOfWork.FolderHeaders.GetFolderTree(folderHeaderId);

            if (folderTree.Any())
            {
                var queue = new Queue<FolderHeader>();
                queue.Enqueue(folderTree[0]);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();

                    foreach (var childFolder in node.ChildFolders)
                    {
                        if (!childFolders.Contains(childFolder.Id))
                            childFolders.Add(childFolder.Id);

                        queue.Enqueue(childFolder);
                    }
                }
            }

            return childFolders;
        }


        //public FolderList GetFolderList()
        //{
        //    FolderList folderList = new FolderList();

        //    foreach (var currentFolder in _unitOfWork.FolderHeaders.GetAll(true))
        //    {
        //        folderList.FolderHeaders.Add(_mapper.Map<FolderHead>(currentFolder));
        //    }

        //    return folderList;
        //}

        public FolderList GetFolderList(Guid folderHeaderId = default(Guid), bool validParents = false)
        {
            FolderList folderList = new FolderList();

            if (folderHeaderId == Guid.Empty)
            {
                foreach (var currentFolder in _unitOfWork.FolderHeaders.GetAll(true))
                {
                    folderList.FolderHeaders.Add(_mapper.Map<FolderHead>(currentFolder));
                }
            }
            else
            {
                var childFolders = new List<Guid>();

                //Check if we only want to return teams that would be applicable as a parent for the passed in TeamHeader Id (i.e. aren't already a child of the TeamHeader passed in)
                if (validParents)
                {
                    childFolders = GetChildFolderIds(folderHeaderId);
                    childFolders.Add(folderHeaderId);
                }

                foreach (var currentFolder in _unitOfWork.FolderHeaders.GetAll(true))
                {
                    if (!childFolders.Contains(currentFolder.Id))
                        folderList.FolderHeaders.Add(_mapper.Map<FolderHead>(currentFolder));
                }
            }

            return folderList;
        }

        //public FolderList GetFoldersForUser(Guid userDetailId)
        //{
        //    throw new NotImplementedException();
        //}

        public FolderStruct GetFolderStructure(Guid folderHeaderId)
        {
            FolderStruct folderStruct = new FolderStruct();
            var folderTree = _unitOfWork.FolderHeaders.GetFolderTree(folderHeaderId);

            if (folderTree.Any())
                folderStruct = _mapper.Map<FolderStruct>(folderTree.First());

            return folderStruct;
        }

        public FolderList GetMainFolders()
        {
            FolderList folderList = new FolderList();

            foreach (var currentFolder in _unitOfWork.FolderHeaders.GetMainFolders())
            {
                folderList.FolderHeaders.Add(_mapper.Map<FolderHead>(currentFolder));
            }

            return folderList;
        }

        //public UserList GetUsersForTeam(Guid teamHeaderId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
