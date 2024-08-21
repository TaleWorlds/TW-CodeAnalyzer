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
                if (upperFound && IsUpperCase(name[i]))
                {
                    currentCandidate += name[i];
                }
                else
                {
                    if (currentCandidate.Length > 2)
                    {
                        currentCandidate = currentCandidate.Substring(0, currentCandidate.Length - 1);
                        _addCandidate(currentCandidate);

                    }
                    currentCandidate = name[i].ToString();
                }
                upperFound = IsUpperCase(name[i]);
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
