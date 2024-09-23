using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker.Conventions
{
    public class TPascalCaseBehaviour: ConventionBehaviour
    {

        public static TPascalCaseBehaviour Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TPascalCaseBehaviour();
                }
                return _instance;
            }

        }

        private static TPascalCaseBehaviour _instance;
        Regex _regexWhole = new Regex("^[T][A-Z](([a-z0-9]+[A-Z]?)*)$");

        public sealed override IReadOnlyList<string> FindWhiteListCandidates(string name)
        {
            var candidates = PascalCaseBehaviour.Instance.FindWhiteListCandidates(name.Substring(1));
            return candidates;

        }

        public sealed override string FixListedItems(string name, HashSet<string> list)
        {
            if (list != null)
            {
                name = name[0] + PascalCaseBehaviour.Instance.FixListedItems(name.Substring(1), list);
            }
            return name;
        }

        public sealed override string FixThis(string name)
        {
            string newName = "";
            if (name.StartsWith("T") || name.StartsWith("t"))
            {
                newName = "T" + PascalCaseBehaviour.Instance.FixThis(name.Substring(1));
            }
            else
            {
                newName = "T" + PascalCaseBehaviour.Instance.FixThis(name);
            }

            return newName;
        }

        public sealed override bool IsMatching(string name)
        {
            name = FixListedItems(name, WhiteListParser.Instance.WhiteListWords);
            return _regexWhole.IsMatch(name);
        }
    }
}
