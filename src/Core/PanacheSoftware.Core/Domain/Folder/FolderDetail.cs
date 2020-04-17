using PanacheSoftware.Core.Domain.Core;
using System;

namespace PanacheSoftware.Core.Domain.Folder
{
    public class FolderDetail : PanacheSoftwareEntity
    {
        public Guid FolderHeaderId { get; set; }
        public virtual FolderHeader FolderHeader { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
