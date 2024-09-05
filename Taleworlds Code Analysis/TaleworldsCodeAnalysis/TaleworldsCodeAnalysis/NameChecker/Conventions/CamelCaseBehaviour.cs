using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker.Conventions
{
    public class CamelCaseBehaviour : ConventionBehaviour
    {
        public static CamelCaseBehaviour Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CamelCaseBehaviour();
                }
                return _instance;
            }
        }

        private static CamelCaseBehaviour _instance;
        private Regex _regexWhole = new Regex("^[a-z](([a-z0-9]*[A-Z]?)*)$");

        public sealed override string FixThis(string name)
        {
            Regex regex = new Regex("[^A-Za-z0-9]");
            name=regex.Replace(name, "");
            
            var whiteIgnoredName=NameCheckerLibrary.GetWhiteListedIgnoreVersion(name);

            string newWord = "";
            if (whiteIgnoredName[0] == name[0])
            {
                newWord += name[0].ToString().ToLower();
            }
            else
            {
                newWord += name[0];
            }
            bool upperCaseFound = NameCheckerLibrary.IsUpperCase(name[0]);

            return FixExceptFirstCharacter(name,whiteIgnoredName,upperCaseFound,newWord);
        }

        public string FixExceptFirstCharacter(string name,string whiteIgnoredName,bool upperCaseFound, string newWord)
        {
            for (int i = 1; i < name.Length; i++)
            {
                if (whiteIgnoredName[i] == '#')
                {
                    newWord += name[i];
                    continue;
                }
                if (upperCaseFound)
                {
                    if (i < name.Length - 1 && !NameCheckerLibrary.IsUpperCase(name[i + 1]))
                    {
                        newWord += name[i];
                    }
                    else
                    {
                        newWord += char.ToLower(name[i]);
                    }
                }
                else
                {
                    newWord += name[i];
                }

                upperCaseFound = NameCheckerLibrary.IsUpperCase(name[i]);
            }
            return newWord;
        }

        public sealed override IReadOnlyList<string> FindWhiteListCandidates(string name)
        {
            string currentCandidate="";
            bool upperFound = false;
            if (NameCheckerLibrary.IsUpperCase(name[0]))
            {
                currentCandidate += name[0];
                upperFound = true;
            }
            return NameCheckerLibrary.OneUpperCaseAllowedCandidates(name,currentCandidate,upperFound);
        }

        public sealed override string FixListedItems(string name,HashSet<string> list)
        {
            if (list==null)
            {
                return name;
            }
            foreach (var item in list)
            {
                Regex regex = new Regex(item);
                var matches=regex.Matches(name);
                foreach (Match match in matches)
                {
                    var firstLetterIndex = match.Index;
                    var lastLetterIndex = match.Index + match.Length-1;
                    if (firstLetterIndex != 0)
                    {
                        name = name.Substring(0, firstLetterIndex) + char.ToUpper(name[firstLetterIndex]) + name.Substring(firstLetterIndex + 1);
                    }
                    else
                    {
                        name = char.ToLower(name[0])+name.Substring(1);
                    }

                    for (int i = firstLetterIndex; i < lastLetterIndex+1; i++)
                    {
                        name = name.Substring(0, i) + char.ToLower(name[i]) + name.Substring(i + 1);
                    }
                }
            }

            return name;
        }

        public sealed override bool IsMatching(string name)
        {
            name = FixListedItems(name,WhiteListParser.Instance.WhiteListWords);
            return _regexWhole.IsMatch(name);
        }
    }
}
