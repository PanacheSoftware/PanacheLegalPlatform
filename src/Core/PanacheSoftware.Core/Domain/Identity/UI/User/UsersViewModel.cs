using System;
using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Identity
{
    public class UsersViewModel
    {
        public UsersViewModel()
        {
            Users = new List<UserViewModel>();
        }

        public List<UserViewModel> Users { get; set; }
    }

    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Status { get; set; }
    }
}
