using System.Threading.Tasks;

namespace PanacheSoftware.UI.Core.Helpers
{
    public interface IRazorPartialToStringRenderer
    {
        Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model);
        string GetStringBetween(string strSource, string strStartTag, string strEndTag);
    }
}
