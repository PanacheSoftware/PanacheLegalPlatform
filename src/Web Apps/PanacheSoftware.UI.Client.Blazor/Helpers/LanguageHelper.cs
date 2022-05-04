using PanacheSoftware.Core.Domain.API.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PanacheSoftware.UI.Client.Blazor.Helpers
{
    public static class LanguageHelper
    {
        public static string GetTitle(LangQueryList langQueryList, long textCode)
        {
            if (langQueryList != null)
            {
                var foundLangQuery = langQueryList.LangQuerys.Find(l => l.TextCode == textCode);

                if (foundLangQuery != null)
                {
                    if (!string.IsNullOrWhiteSpace(foundLangQuery.Text))
                        return foundLangQuery.Text;
                }
            }

            return string.Empty;
        }
    }
}
