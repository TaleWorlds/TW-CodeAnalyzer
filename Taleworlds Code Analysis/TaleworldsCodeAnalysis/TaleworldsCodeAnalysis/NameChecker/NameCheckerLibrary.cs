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
            string pattern = "^[_][a-zA-Z]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(name);
        }

        public static bool IsPascalCase(string name)
        {
            string pattern = "^[A-Z](([a-z0-9]+[A-Z]?)*)$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(name);
        }

    }
}
