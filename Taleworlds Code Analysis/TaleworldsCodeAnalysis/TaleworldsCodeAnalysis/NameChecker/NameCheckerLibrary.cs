using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TaleworldsCodeAnalysis.NameChecker
{
    public static class NameCheckerLibrary
    {
        private const string _pascalRegex = "^[A-Z](([a-z0-9]+[A-Z]?)*)$";
        private const string _iPascalRegex = "^[I][A-Z](([a-z0-9]+[A-Z]?)*)$";
        private const string _tPascalRegex = "^[T][A-Z](([a-z0-9]+[A-Z]?)*)$";
        private const string _pascalSingleRegex = "[A-Z][a-z0-9]+";
        private const string _underScoreRegex = "^[_][a-z]*([a-z0-9]+[A-Z]?)*$";
        private const string _underScoreBeginningSingleRegex = "^[_][a-z0-9]+";
        private const string _camelRegex = "^[a-z](([a-z0-9]+[A-Z]?)*)$";
        private const string _camelBeginningSingleRegex = "^[a-z0-9]+";

        public static bool IsMatchingConvention(string name, ConventionType type)
        {
            name = _removeWhiteListItems(name, type);
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
                case ConventionType.IPascalCase:
                    pattern = _iPascalRegex;
                    break;
                case ConventionType.TPascalCase:
                    pattern = _tPascalRegex;
                    break;
            }
            Regex regex = new Regex(pattern);
            return regex.IsMatch(name);
        }

        private static string _removeWhiteListItems(string name,ConventionType conventionType)
        {
            return _removeWords(name, WhiteListParser.Instance.WhiteListWords, conventionType);
        }

        private static string _returnAfterReplace(string name, string patternParameter)
        {
            Regex regex = new Regex(patternParameter);
            name = regex.Replace(name, "0");
            return name;
        }

        public static IReadOnlyList<string> GetNewWhiteListItemsToFix(string name,ConventionType type)
        {
            string originalName = name;
            name=_removeWhiteListItems(name,type);
            var forbiddenWords = new List<string>();
            switch (type)
            {
                case ConventionType.camelCase:
                    name=_returnAfterReplace(name,_camelBeginningSingleRegex);
                    break;
                case ConventionType._uscoreCase:
                    if (!name.StartsWith("_"))
                    {
                        return forbiddenWords;
                    }
                    name=_returnAfterReplace(name, _underScoreBeginningSingleRegex);
                    break;
                case ConventionType.PascalCase:
                    name=_returnAfterReplace(name,"^" + _pascalSingleRegex);
                    break;
                case ConventionType.IPascalCase:
                    if (!name.StartsWith("I"))
                    {
                        return forbiddenWords;
                    }
                    name = name.Substring(1);
                    name = _returnAfterReplace(name, "^" + _pascalSingleRegex);
                    break;
                case ConventionType.TPascalCase:
                    if (!name.StartsWith("T"))
                    {
                        return forbiddenWords;
                    }
                    name =name.Substring(1);
                    name = _returnAfterReplace(name, "^" + _pascalSingleRegex);
                    break;
            }

            name = _returnAfterReplace(name, _pascalSingleRegex);
            
            string currentWord="";

            void AddWord(string word)
            {
                if (!forbiddenWords.Contains(currentWord))
                {
                    if(currentWord.Length>1)
                    {
                        forbiddenWords.Add(currentWord);
                    }
                    currentWord = "";
                }
            }

            foreach (var item in name)
            {
                if(item!='0' && item!='_')
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

            if(_isItOkeyWithNewWhiteWords(originalName,forbiddenWords,type))
            {
                return forbiddenWords;
            }

            return new List<string>();

        }

        private static bool _isItOkeyWithNewWhiteWords(string name,IReadOnlyList<string> newWords, ConventionType conventionType)
        {
            IReadOnlyList<string> newWordList= newWords.Concat(WhiteListParser.Instance.WhiteListWords).ToList();
            return IsMatchingConvention(_removeWords(name, newWordList, conventionType), conventionType); ;
        }

        private static string _removeWords(string name, IReadOnlyList<string> whiteListedWords, ConventionType conventionType)
        {
            var currentWord = name;
            string firstLetter = "";

            if (conventionType == ConventionType.IPascalCase || conventionType == ConventionType.TPascalCase)
            {
                currentWord = currentWord.Substring(1);
                firstLetter = name[0].ToString();
            }

            foreach (var white in whiteListedWords)
            {
                Regex regex = new Regex(white);
                currentWord = regex.Replace(currentWord, "");
            }
            return firstLetter + currentWord;
        }

        public static string GetWhiteListedIgnoreVersion(string name)
        {
            string ignorePattern = "";
            foreach (var item in WhiteListParser.Instance.WhiteListWords)
            {
                ignorePattern += "" + item + "+|";
            }
            ignorePattern = ignorePattern.TrimEnd('|');
            Regex regex = new Regex(ignorePattern);
            var matchCollection = regex.Matches(name);
            var whiteIgnoredName = name;
            foreach (Match match in matchCollection)
            {
                for (int i = 0; i < match.Length; i++)
                {
                    whiteIgnoredName = whiteIgnoredName.Substring(0, match.Index) + new string('#', match.Length) + whiteIgnoredName.Substring(match.Index + match.Length);
                }
            }
            return whiteIgnoredName;
        }

        public static List<string> OneUpperCaseAllowedCandidates(string name,string currentCandidate, bool upperFound)
        {
            List<string> candidates = new List<string>();
            for (int i = 1; i < name.Length; i++)
            {
                if (upperFound && NameCheckerLibrary.IsUpperCase(name[i]))
                {
                    currentCandidate += name[i];
                }
                else
                {
                    if (currentCandidate.Length > 1)
                    {
                        currentCandidate = currentCandidate.Substring(0, currentCandidate.Length - 1);
                        _addCandidate(currentCandidate);

                    }
                    currentCandidate = name[i].ToString();
                }
                upperFound = NameCheckerLibrary.IsUpperCase(name[i]);
            }

            void _addCandidate(string candidate)
            {
                if (!candidates.Contains(candidate))
                {
                    candidates.Add(candidate);
                }
            }

            if (currentCandidate.Length > 1)
            {
                currentCandidate = currentCandidate.Substring(0, currentCandidate.Length);
                _addCandidate(currentCandidate);
            }

            foreach (string word in WhiteListParser.Instance.WhiteListWords)
            {
                candidates.Remove(word);
            }
            return candidates;
        }

        public static bool IsUpperCase(char c)
        {
            return c == char.ToUpper(c);
        }

        public static bool IsLowerCase(char c)
        {
            return c == char.ToLower(c);
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
