using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PanacheSoftware.UI.Core.Helpers
{
    public static class GeneralHelpers
    {
        public static IEnumerable<string> SeperatedListToEnumerable(string seperatedList, string seperator = ",", string prefix = default(string), string suffix = default(string))
        {
            var enumerableResult = seperatedList.Split(seperator).Select(s => s.Trim()).ToList();

            if(enumerableResult.Any())
            {
                if(!string.IsNullOrWhiteSpace(prefix) || !string.IsNullOrWhiteSpace(suffix))
                {
                    for (int iCount = 0; iCount < enumerableResult.Count; iCount++)
                    {
                        enumerableResult[iCount] = $"{prefix}{enumerableResult[iCount]}{suffix}";
                    }
                }

                return enumerableResult;
            }

            return new List<string>();
        }
    }
}
