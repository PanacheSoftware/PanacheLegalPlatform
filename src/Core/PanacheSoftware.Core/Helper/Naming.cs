using System.Text.RegularExpressions;

namespace PanacheSoftware.Core.Helper
{
    public static class Naming
    {
        public static string CreateFieldShortName(string longName, int counter = 0)
        {
            if (string.IsNullOrWhiteSpace(longName))
                return string.Empty;

            var shortName = longName.ToUpper();
            shortName = Regex.Replace(shortName, "[^A-Z0-9_.]+", "", RegexOptions.Compiled);

            var counterLength = counter <= 1 ? 0 : counter.ToString().Length;
            var maxLength = 100 - counterLength;

            if (shortName.Length > maxLength)
                shortName = shortName.Substring(0, maxLength);

            if (counter > 0)
                shortName = $"{shortName}{counter}";

            return shortName;
        }
    }
}
