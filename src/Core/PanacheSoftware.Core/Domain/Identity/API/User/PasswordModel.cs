using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.Identity.API
{
    public class PasswordModel
    {
        public PasswordModel()
        {
            UserName = string.Empty;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            NewPasswordConfirm = string.Empty;
        }

        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string CurrentPassword { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string NewPassword { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string NewPasswordConfirm { get; set; }
    }
}
