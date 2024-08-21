using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker.Conventions
{
    public class PascalCaseBehaviour : ConventionBehaviour
    {
        public static UnderScoreCaseBehaviour Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UnderScoreCaseBehaviour();
                }
                return _instance;
            }

        }

        private static UnderScoreCaseBehaviour _instance;
        private Regex _regexWhole = new Regex("^[A-Z](([a-z0-9]+[A-Z]?)*)$");

        public override IReadOnlyList<string> FindWhiteListCandidates(string name)
        {
            List<string> candidates = new List<string>();

            string currentCandidate = "";
            bool upperFound = true;
            if (NameCheckerLibrary.IsLowerCase(name[0]))
            {
                currentCandidate += name[0];
                upperFound = false;
            }
            return NameCheckerLibrary.OneUpperCaseAllowedCandidates(name, currentCandidate, upperFound);
        }

        public override string FixListedItems(string name, IReadOnlyList<string> list)
        {
            foreach (var item in list)
            {
                Regex regex = new Regex(item);
                var matches = regex.Matches(name);
                foreach (Match match in matches)
                {
                    var firstLetterIndex = match.Index;
                    var lastLetterIndex = match.Index + match.Length - 1;
                    if (firstLetterIndex != 0)
                    {
                        name = name.Substring(0, firstLetterIndex) + 
                            char.ToUpper(name[firstLetterIndex]) + 
                            name.Substring(firstLetterIndex + 1);
                    }
                    else
                    {
                        name = char.ToUpper(name[0]) + name.Substring(1);
                    }

                    for (int i = firstLetterIndex; i < lastLetterIndex + 1; i++)
                    {
                        name = name.Substring(0, i) + char.ToLower(name[i]) + name.Substring(i + 1);
                    }
                }
            }

            return name;
        }

        public override string FixThis(string name)
        {
            Regex regex = new Regex("[^A-Za-z0-9]");
            name = regex.Replace(name, "");

            var whiteIgnoredName = NameCheckerLibrary.GetWhiteListedIgnoreVersion(name);

            string newWord = "";
            if (whiteIgnoredName[0] == name[0])
            {
                newWord += name[0].ToString().ToUpper();
            }
            else
            {
                newWord += name[0];
            }
            bool upperCaseFound = NameCheckerLibrary.IsUpperCase(name[0]);
            return CamelCaseBehaviour.Instance.FixExceptFirstCharacter(name, whiteIgnoredName, upperCaseFound, newWord);
        }

        public override bool IsMatching(string name)
        {
            name = FixListedItems(name, WhiteListParser.Instance.WhiteListWords);
            return _regexWhole.IsMatch(name);
        }
    }
}
