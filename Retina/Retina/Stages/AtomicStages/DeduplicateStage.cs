using System.Collections.Generic;
using System.Linq;
using System.IO;
using Retina.Configuration;
using Retina.Extensions;
using System.Text;
using System;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    class DeduplicateStage : AtomicStage
    {
        public DeduplicateStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Maybe a numeric parameter to keep multiple copies?
            var matchSets = new Dictionary<string, List<Match>>();
            
            foreach(var m in Matches)
            {
                string key = m.Replacement;
                if (!matchSets.ContainsKey(key))
                    matchSets[m.Replacement] = new List<Match>();

                matchSets[m.Replacement].Add(m.Match);
                
            }

            var toDelete = new HashSet<int>();
            foreach (var matchSet in matchSets)
            {
                List<Match> matches = matchSet.Value;

                if (Config.Random)
                    matches.RemoveAt(Random.RNG.Next(matches.Count));
                else if (Config.Reverse)
                    matches.RemoveAt(matches.Count - 1);
                else
                    matches.RemoveAt(0);

                foreach (var m in matches)
                {
                    for (int i = 0; i < m.Length; ++i)
                    {
                        if (Config.GetLimit(1).IsInRange(i, m.Length))
                            toDelete.Add(i + m.Index);
                    }
                }
            }
                        
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
