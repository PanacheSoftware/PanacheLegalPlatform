using PanacheSoftware.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.Identity.API
{
    public class UserModel
    {
        public UserModel()
        {
            Id = Guid.Empty;
            //UserName = string.Empty;
            FirstName = string.Empty;
            Surname = string.Empty;
            FullName = string.Empty;
            Base64ProfileImage = Base64Images.PanacheSoftwareDot;
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now.AddYears(1);
            Status = StatusTypes.Open;
            Description = string.Empty;
        }

        [Required]
        public Guid Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        //public string UserName { get; set; }
        //[Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Surname { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string FullName { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Base64ProfileImage { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Must be a valid Email address")]
        public string Email { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }
        [Required, DataType(DataType.DateTime, ErrorMessage = "Must be a valid DateTime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Status { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }
    }
}
