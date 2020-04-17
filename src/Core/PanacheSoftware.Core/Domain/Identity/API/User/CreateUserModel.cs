using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PanacheSoftware.Core.Domain.Identity.API
{
    public class CreateUserModel : UserModel
    {
        public CreateUserModel()
        {
            Password = string.Empty;
            PasswordConfirm = string.Empty;
        }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string PasswordConfirm { get; set; }
    }
}
