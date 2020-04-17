using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Core.Domain.Join;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Folder
{
    public class FolderHeader : PanacheSoftwareEntity
    {
        public FolderHeader()
        {
            FolderDetail = new FolderDetail();
            ChildFolders = new HashSet<FolderHeader>();
            ChildNodes = new HashSet<FolderNode>();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public Guid? ParentFolderId { get; set; }
        public Guid MainUserId { get; set; }
        public Guid TeamHeaderId { get; set; }
        public Guid ClientHeaderId { get; set; }
        public virtual FolderHeader ParentFolder { get; set; }

        public virtual FolderDetail FolderDetail { get; set; }
        public virtual ICollection<FolderHeader> ChildFolders { get; set; }
        public virtual ICollection<FolderNode> ChildNodes { get; set; }
        public int SequenceNumber { get; set; }

        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }
        public bool Completed { get; set; }
    }
}
