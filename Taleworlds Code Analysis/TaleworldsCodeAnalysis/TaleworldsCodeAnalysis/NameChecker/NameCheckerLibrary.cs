using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    public static class NameCheckerLibrary
    {
        private const string _pascalRegex = "^[A-Z](([a-z0-9]+[A-Z]?)*)$";
        private const string _pascalSingleRegex = "[A-Z][a-z0-9]+";
        private const string _underScoreRegex = "^[_][a-z]*([a-z0-9]+[A-Z]?)*$";
        private const string _underScoreBeginningSingleRegex = "^[_][a-z0-9]+";
        private const string _camelRegex = "^[a-z](([a-z0-9]*[A-Z]?)*)$";
        private const string _camelBeginningSingleRegex = "^[a-z0-9]+";

        public static bool IsMatchingConvention(string name, ConventionType type)
        {
            name = _removeWhiteListItems(name);
            string pattern="";
            switch(type)
            {
                case ConventionType.camelCase:
                    pattern = _camelRegex;
                    break;
                case ConventionType._uscoreCase:
                    pattern = _underScoreRegex;
                    break;
                case ConventionType.PascalCase:
                    pattern = _pascalRegex;
                    break;
            }
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

        public static IReadOnlyList<string> GetForbiddenPieces(string name,ConventionType type)
        {
            string originalName = name;
            name=_removeWhiteListItems(name);
            string pattern = "";
            var forbiddenWords = new List<string>();
            Regex regex;
            switch (type)
            {
                case ConventionType.camelCase:
                    pattern = _camelBeginningSingleRegex;
                    regex = new Regex(pattern);
                    name=regex.Replace(name, "0");
                    break;
                case ConventionType._uscoreCase:
                    pattern = _underScoreBeginningSingleRegex;
                    regex = new Regex(pattern);
                    name=regex.Replace(name, "0");
                    break;
                case ConventionType.PascalCase:
                    pattern = "^"+_pascalSingleRegex;
                    regex = new Regex(pattern);
                    name = regex.Replace(name, "0");
                    break;   
            }

            pattern = _pascalSingleRegex;
            regex = new Regex(pattern);
            name=regex.Replace(name, "0");
            
            string currentWord="";

            void AddWord(string word)
            {
                if (!forbiddenWords.Contains(currentWord))
                {
                    forbiddenWords.Add(currentWord);
                    currentWord = "";
                }
            }

            foreach (var item in name)
            {
                if(item!='0')
                {
                    currentWord += item;
                }
                else if(currentWord!="")
                {
                    AddWord(currentWord);
                }
            }

            if (currentWord != "")
            {
                AddWord(currentWord);
            }

            if(forbiddenWords.Contains(originalName))
            {
                forbiddenWords.Remove(originalName);
            }
            return forbiddenWords;
        }

        public static ConventionType stringToConventionType(string type)
        {
            switch (type)
            {
                case "camelCase":
                    return ConventionType.camelCase;
                case "_uscoreCase":
                    return ConventionType._uscoreCase;
                case "PascalCase":
                    return ConventionType.PascalCase;
                case "IPascalCase":
                    return ConventionType.IPascalCase;
                case "TPascalCase":
                    return ConventionType.TPascalCase;
                default:
                    throw new Exception("Invalid convention type");
            }
        }
    }
}
