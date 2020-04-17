using PanacheSoftware.Core.Domain.Core;
using System;

namespace PanacheSoftware.Core.Domain.Folder
{
    public class FolderNodeDetail : PanacheSoftwareEntity
    {
        public Guid FolderNodeId { get; set; }
        public virtual FolderNode FolderNode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Data { get; set; }
    }
}
