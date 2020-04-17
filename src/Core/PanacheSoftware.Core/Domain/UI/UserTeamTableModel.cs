using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Language;

namespace PanacheSoftware.Core.Domain.UI
{
    public class UserTeamTableModel
    {
        public SelectList StatusList { get; set; }
        public SelectList TeamSelectList { get; set; }
        public UserProfileModel userProfileModel { get; set; }
        public string[] teamListRows { get; set; }
        public LangQueryList langQueryList { get; set; }
    }
}
