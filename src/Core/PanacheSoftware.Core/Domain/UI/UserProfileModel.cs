using PanacheSoftware.Core.Domain.API.Join;
using PanacheSoftware.Core.Domain.Identity.API;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PanacheSoftware.Core.Domain.UI
{
    public class UserProfileModel
    {
        public UserProfileModel()
        {
            userTeamJoins = new List<UserTeamJoin>();
        }
        public UserModel userModel { get; set; }
        public List<UserTeamJoin> userTeamJoins { get; set; }
        [DataType(DataType.Password)]
        public string password { get; set; }
        [DataType(DataType.Password)]
        public string passwordConfirm { get; set; }
    }
}
