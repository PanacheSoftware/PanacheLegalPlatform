using PanacheSoftware.Core.Domain.API.Language;
using PanacheSoftware.Core.Domain.API.Settings;
using PanacheSoftware.Core.Domain.UI;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Core.PageModel
{
    public interface IPanacheSoftwarePageModel
    {
        LangQueryList langQueryList { get; set; }
        string SaveState { get; set; }
        string ErrorString { get; set; }
        SaveMessageModel SaveMessageModel { get; set; }
        Task<bool> PageConstructor(string saveState, string accessToken);
    }
}
