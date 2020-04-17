using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Core.Domain.Folder;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Join
{
    public class TeamFolder : PanacheSoftwareEntity
    {
        public Guid TeamHeaderId { get; set; }
        public Guid FolderHeaderId { get; set; }
        public virtual FolderHeader FolderHeader { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
