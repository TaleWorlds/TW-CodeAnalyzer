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
            name = _removeWhiteListItems(name);
            string pattern = "^[_][a-z]*([a-z0-9]+[A-Z]?)*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(name);
        }

        public static bool IsPascalCase(string name)
        {
            name = _removeWhiteListItems(name);
            string pattern = "^[A-Z](([a-z0-9]+[A-Z]?)*)$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(name);
        }

        public static bool IsCamelCase(string name)
        {
            name = _removeWhiteListItems(name);
            string pattern = "^[a-z](([a-z0-9]+[A-Z]?)*)$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(name);
        }


        private static string _removeWhiteListItems(string name)
        {
            foreach(var white in WhiteListParser.Instance.WhiteListWords)
            {
                Regex regex = new Regex(white);
                name = regex.Replace(name,"");
            }
            return name;
        }
    }
}
