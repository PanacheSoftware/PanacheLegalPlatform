using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.API.Language
{
    public class LangItem
    {
        public LangItem()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddYears(1);
            Status = StatusTypes.Open;
        }

        public Guid Id { get; set; }
        public Guid LanguageHeaderId { get; set; }

        [Required]
        [Display(Name = "Language Code")]
        public string LanguageCodeId { get; set; }
        [Required]
        [Display(Name = "Text")]
        public string Text { get; set; }

        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
