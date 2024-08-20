using System;
using System.Collections.Generic;
using System.Text;

namespace TaleworldsCodeAnalysis.NameChecker.Conventions
{
    public abstract class ConventionBehaviour
    {
        public abstract bool IsMatching(string name);

        public abstract string FixThis(string name);

        public abstract string FixListedItems(string name,IReadOnlyList<string> list);

        public abstract IReadOnlyList<string> FindWhiteListCandidates(string name);
    }
}
