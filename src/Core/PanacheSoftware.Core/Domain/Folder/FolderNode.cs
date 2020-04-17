using PanacheSoftware.Core.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.Folder
{
    public class FolderNode : PanacheSoftwareEntity
    {
        public FolderNode()
        {
            FolderNodeDetail = new FolderNodeDetail();
        }

        public Guid FolderHeaderId { get; set; }
        public virtual FolderHeader FolderHeader { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }

        public DateTime CompletionDate { get; set; }
        public DateTime OriginalCompletionDate { get; set; }
        public DateTime CompletedOnDate { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string NodeType { get; set; }

        public bool Completed { get; set; }
        
        public int SequenceNumber { get; set; }

        public virtual FolderNodeDetail FolderNodeDetail { get; set; }
    }
}
