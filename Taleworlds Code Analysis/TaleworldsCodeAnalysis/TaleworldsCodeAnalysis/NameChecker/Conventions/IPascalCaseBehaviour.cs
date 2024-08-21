using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TaleworldsCodeAnalysis.NameChecker.Conventions
{
    public class IPascalCaseBehaviour : ConventionBehaviour
    {

        public static IPascalCaseBehaviour Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IPascalCaseBehaviour();
                }
                return _instance;
            }

        }

        private static IPascalCaseBehaviour _instance;
        Regex _regexWhole = new Regex("^[I][A-Z](([a-z0-9]+[A-Z]?)*)$");

        public override IReadOnlyList<string> FindWhiteListCandidates(string name)
        {
            var candidates = PascalCaseBehaviour.Instance.FindWhiteListCandidates(name.Substring(1));
            return candidates;

        }

        public override string FixListedItems(string name, IReadOnlyList<string> list)
        {
            name = name[0] + PascalCaseBehaviour.Instance.FixListedItems(name.Substring(1), list);
            return name;
        }

        public override string FixThis(string name)
        {
            string newName = "";
            if (name.StartsWith("I") || name.StartsWith("i"))
            {
                newName = "I" + PascalCaseBehaviour.Instance.FixThis(name.Substring(1));
            }
            else
            {
                newName = "I" + PascalCaseBehaviour.Instance.FixThis(name);
            }

            return newName;
        }

        public override bool IsMatching(string name)
        {
            name = FixListedItems(name, WhiteListParser.Instance.WhiteListWords);
            return _regexWhole.IsMatch(name);
        }
    }
}
