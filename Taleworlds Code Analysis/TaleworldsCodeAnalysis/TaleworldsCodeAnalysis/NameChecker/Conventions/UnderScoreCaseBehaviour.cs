using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker.Conventions
{
    public class UnderScoreCaseBehaviour : ConventionBehaviour
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
        private Regex _regexWhole = new Regex("^[_][a-z]*([a-z0-9]+[A-Z]?)*$");
        public sealed override string FixThis(string name)
        {
            string newName = "";
            if(!name.StartsWith("_"))
            {
                newName = "_"+CamelCaseBehaviour.Instance.FixThis(name);
            }
            else
            {
                newName = "_" + CamelCaseBehaviour.Instance.FixThis(name.Substring(1));
            }

            return newName;
        }

        public sealed override string FixListedItems(string name,HashSet<string> list)
        {
            if (list != null)
            {
                name = name[0] + CamelCaseBehaviour.Instance.FixListedItems(name.Substring(1), list);
            }
            return name;
        }

        public sealed override bool IsMatching(string name)
        {
            name = FixListedItems(name,WhiteListParser.Instance.WhiteListWords);
            return _regexWhole.IsMatch(name);
        }

        public sealed override IReadOnlyList<string> FindWhiteListCandidates(string name)
        {
            IReadOnlyList<string> candidates = new List<string>();
            if (name.StartsWith("_") && name != "_")
            {
                candidates = CamelCaseBehaviour.Instance.FindWhiteListCandidates(name.Substring(1));
            }
            return candidates;
        }
    }
}
