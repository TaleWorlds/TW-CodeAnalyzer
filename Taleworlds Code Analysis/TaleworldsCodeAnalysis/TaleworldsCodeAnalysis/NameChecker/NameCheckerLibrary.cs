using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    public static class NameCheckerLibrary
    {
        private const string _pascalRegex = "^[A-Z](([a-z0-9]+[A-Z]?)*)$";
        private const string _underScoreRegex = "^[_][a-z]*([a-z0-9]+[A-Z]?)*$";
        private const string _camelRegex = "^[a-z](([a-z0-9]*[A-Z]?)*)$";

        public static bool IsUnderScoreCase(string name)
        {
            name = _removeWhiteListItems(name);
            string pattern = _underScoreRegex;
            Regex regex = new Regex(pattern);
            return regex.IsMatch(name);
        }

        public static bool IsPascalCase(string name)
        {
            name = _removeWhiteListItems(name);
            string pattern = _pascalRegex;
            Regex regex = new Regex(pattern);

            return regex.IsMatch(name);
        }

        public static bool IsCamelCase(string name)
        {
            name = _removeWhiteListItems(name);
            string pattern = _camelRegex;
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

        private static IReadOnlyList<string> GetForbiddenPieces(string name)
        {
            string pattern = "[A-Z][a-z0-9]+";
            Regex regex = new Regex(pattern);
            regex.Replace(name, "0");
            var forbiddenWords = new List<string>();
            string currentWord="";
            foreach (var item in name)
            {
                if(item!='0')
                {
                    currentWord += item;
                }
                else
                {
                    forbiddenWords.Add(currentWord);
                    currentWord = "";
                }
            }

            return forbiddenWords;
        }
    }
}
