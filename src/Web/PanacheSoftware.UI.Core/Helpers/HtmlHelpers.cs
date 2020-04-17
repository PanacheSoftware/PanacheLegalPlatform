using Microsoft.AspNetCore.Mvc.Rendering;
using PanacheSoftware.Core.Domain.API.Language;

namespace PanacheSoftware.UI.Core.Helpers
{
    public static class HtmlHelpers
    {

        public static string GetTitle(this IHtmlHelper html, LangQueryList langQueryList, long textCode)
        {
            if(langQueryList != null)
            {
                var foundLangQuery = langQueryList.LangQuerys.Find(l => l.TextCode == textCode);

                if(foundLangQuery != null)
                {
                    if (!string.IsNullOrWhiteSpace(foundLangQuery.Text))
                        return foundLangQuery.Text;
                }
            }

            return string.Empty;
        }

        public static string IsSelected(this IHtmlHelper html, string root = null, string page = "", string cssClass = "", string cssClassAlt = "")
        {
            string currentRoute = (string)html.ViewContext.RouteData.Values["page"];

            GetCurrentRootAndPage(currentRoute, out string currentRoot, out string currentPage);

            if (root == currentRoot)
            {
                if (!string.IsNullOrWhiteSpace(page))
                {
                    if (page == currentPage)
                    {
                        return cssClass;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    if (root == currentRoot)
                    {
                        return cssClass;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            return cssClassAlt;
        }

        private static void GetCurrentRootAndPage(string routeDataPage, out string root, out string page)
        {
            page = string.Empty;
            root = string.Empty;

            if (!string.IsNullOrWhiteSpace(routeDataPage))
            {
                string[] splitString = routeDataPage.Split("/");

                if (splitString.Length >= 2)
                {
                    root = splitString[1];

                    if (splitString.Length > 2)
                    {
                        page = splitString[2];
                    }
                }
            }
        }
    }
}
