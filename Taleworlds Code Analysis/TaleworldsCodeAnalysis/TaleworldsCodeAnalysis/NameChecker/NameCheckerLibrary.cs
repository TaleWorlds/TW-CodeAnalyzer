using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    public static class NameCheckerLibrary
    {
        public static bool IsUnderScoreCase(string name)
        {
            if (!name.StartsWith("_", StringComparison.Ordinal))
            {
                return false;
            }
            else if (name.ToCharArray()[1] != name.ToLower().ToCharArray()[1])
            {
                return false;
            }

            return true;
        }

        public static bool IsPascalCase(string name)
        {
            string pattern = "/^[A-Z][A-Za-z]*$/";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(name);
        }

    }
}
