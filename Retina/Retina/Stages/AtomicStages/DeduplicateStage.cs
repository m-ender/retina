using System.Collections.Generic;
using System.Linq;
using System.IO;
using Retina.Configuration;
using Retina.Extensions;
using System.Text;

namespace Retina.Stages
{
    class DeduplicateStage : AtomicStage
    {
        public DeduplicateStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Random option?
            // - Maybe a numeric parameter to keep multiple copies?
            // - Limit on characters to be deleted.
            var stringSet = new HashSet<string>();
            var toDelete = new HashSet<int>();

            IEnumerable<MatchContext> matches = Matches;
            if (Config.Reverse)
                matches = matches.Reverse();

            foreach(var m in matches)
            {
                if (stringSet.Contains(m.Replacement))
                {
                    for (int i = 0; i < m.Match.Length; ++i)
                    {
                        toDelete.Add(i + m.Match.Index);
                    }
                }
                stringSet.Add(m.Replacement);
            };
                        
            var sortedDeletions = toDelete.ToList();
            sortedDeletions.Sort();
            sortedDeletions.Reverse();

            var mutableInput = new StringBuilder(input);
            foreach (int i in sortedDeletions)
                mutableInput.Remove(i, 1);

            return mutableInput.ToString();
        }
    }
}
