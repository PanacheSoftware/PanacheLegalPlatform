using PanacheSoftware.Core.Domain.API.Language;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Core.PageModel
{
    public interface IPanacheSoftwarePageModel
    {
        LangQueryList langQueryList { get; set; }
        string SaveState { get; set; }
        string ErrorString { get; set; }
        Task<bool> PageConstructor(string saveState, string accessToken);
    }
}
