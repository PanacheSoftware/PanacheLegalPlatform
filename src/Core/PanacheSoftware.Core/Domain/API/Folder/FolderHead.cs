using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.API.Folder
{
    public class FolderHead
    {
        public FolderHead()
        {
            ChildFolders = new List<FolderHead>();
            ChildNodes = new List<FolderNod>();
            FolderDetail = new FolderDet();
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
            CompletionDate = DateTime.Now;
            OriginalCompletionDate = DateTime.Now;
            CompletedOnDate = DateTime.Parse("01/01/1900");
            Completed = false;
        }

        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Folder ID")]
        public string ShortName { get; set; }
        [Required]
        [Display(Name = "Folder Name")]
        public string LongName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        public Guid? ParentFolderId { get; set; }
        public Guid MainUserId { get; set; }
        public Guid TeamHeaderId { get; set; }
        public Guid ClientHeaderId { get; set; }
        public FolderDet FolderDetail { get; set; }
        public List<FolderHead> ChildFolders { get; set; }
        public List<FolderNod> ChildNodes { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int SequenceNumber { get; set; }

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
        public bool Completed { get; set; }
    }
}
