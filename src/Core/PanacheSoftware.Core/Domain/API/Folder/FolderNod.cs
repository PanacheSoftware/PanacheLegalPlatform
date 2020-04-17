using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Folder
{
    public class FolderNod
    {
        public FolderNod()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            CompletionDate = DateTime.Now;
            OriginalCompletionDate = DateTime.Now;
            CompletedOnDate = DateTime.Parse("01/01/1900");
            Status = StatusTypes.Open;
            FolderNodeDetail = new FolderNodDet();
            NodeType = NodeTypes.Task;
            Completed = false;
        }

        public Guid Id { get; set; }
        public Guid FolderHeaderId { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        [Required]
        public string Status { get; set; }

        public string Description { get; set; }
        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CompletionDate { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OriginalCompletionDate { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CompletedOnDate { get; set; }

        [Required]
        public string NodeType { get; set; }

        [Required]
        public bool Completed { get; set; }

        [Required]
        public int SequenceNumber { get; set; }

        public FolderNodDet FolderNodeDetail { get; set; }
    }
}
