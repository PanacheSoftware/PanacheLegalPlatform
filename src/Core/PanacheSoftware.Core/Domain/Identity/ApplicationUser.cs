using Microsoft.AspNetCore.Identity;
using PanacheSoftware.Core.Types;
using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid LastUpdateBy { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public string Base64ProfileImage { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Description { get; set; }
    }
}
