using System.Collections.Generic;

namespace PanacheSoftware.Core.Domain.Identity.API
{
    public class UserListModel
    {
        public UserListModel()
        {
            Users = new List<UserModel>();
        }

        public List<UserModel> Users { get; set; }
    }
}
