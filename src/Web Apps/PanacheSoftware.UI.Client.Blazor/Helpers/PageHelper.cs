using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Client.Blazor.Helpers
{
    public static class PageHelper
    {
        public static string IsSelected(string currentRoute, string root = null, string page = "", string cssClass = "", string cssClassAlt = "")
        {
            //string currentRoute = (string)html.ViewContext.RouteData.Values["page"];

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