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

        private Regex _regexWhole = new Regex("^[a-z](([a-z0-9]+[A-Z]?)*)$");
        public override string FixThis(string name)
        {
            Regex regex = new Regex("[^A-Za-z0-9]");
            name=regex.Replace(name, "");
            string newWord = "";
            newWord += name[0].ToString().ToLower();

            bool upperCaseFound = _isUpperCase(name[0]);
            string ignorePattern = "";
            foreach (var item in WhiteListParser.Instance.WhiteListWords)
            {
                ignorePattern += "" + item + "+|";
            }
            ignorePattern=ignorePattern.TrimEnd('|');
            regex = new Regex(ignorePattern);
            var matchCollection=regex.Matches(name);
            var whiteIgnoredName = name;
            foreach (Match match in matchCollection)
            {
                for (int i = 0; i < match.Length; i++)
                {
                    whiteIgnoredName = whiteIgnoredName.Substring(0,match.Index)+new string('#',match.Length)+whiteIgnoredName.Substring(match.Index+match.Length);
                }
            }

            for (int i = 1; i < name.Length; i++)
            {
                if (whiteIgnoredName[i]=='#')
                {
                    newWord += name[i];
                    continue;
                }
                if (upperCaseFound)
                {
                    if(i<name.Length-1 && !_isUpperCase(name[i+1]))
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

                upperCaseFound = _isUpperCase(name[i]);
            }

            return newWord;
        }

        public override IReadOnlyList<string> FindWhiteListCandidates(string name)
        {
            List<string> candidates = new List<string>();

            string currentCandidate="";
            bool upperFound = false;
            if (_isUpperCase(name[0]))
            {
                currentCandidate += name[0];
                upperFound = true;
            }
            for (int i = 1; i < name.Length; i++)
            {
                if (upperFound && _isUpperCase(name[i]))
                {
                    currentCandidate += name[i];
                }
                else
                {
                    if(currentCandidate.Length>1)
                    {
                        currentCandidate = currentCandidate.Substring(0, currentCandidate.Length - 1);
                        _addCandidate(currentCandidate);
                        
                    }
                    currentCandidate = name[i].ToString(); 
                }
                upperFound = _isUpperCase(name[i]);
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

        private bool _isUpperCase(char c)
        {
            return c == char.ToUpper(c);
        }

        public override string FixListedItems(string name,IReadOnlyList<string> list)
        {
            foreach (var whiteListedItem in WhiteListParser.Instance.WhiteListWords)
            {
                Regex regex = new Regex(whiteListedItem);
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

        public override bool IsMatching(string name)
        {
            name = FixListedItems(name,WhiteListParser.Instance.WhiteListWords);
            return _regexWhole.IsMatch(name);
        }
    }
}
