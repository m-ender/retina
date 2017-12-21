using System.Collections.Generic;
using System.Linq;
using System.IO;
using Retina.Configuration;
using Retina.Extensions;

namespace Retina.Stages
{
    class DeduplicateStage : AtomicStage
    {
        public DeduplicateStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Reverse option
            // - Random option?
            // - Maybe a numeric parameter to keep multiple copies?
            var stringSet = new HashSet<string>();

            var values = Matches.Select(m =>
                {
                    var result = stringSet.Contains(m.Replacement) ? "" : m.Match.Value;
                    stringSet.Add(m.Replacement);
                    return result;
                });

            var separators = Separators.Select(s => s.Match.Value);


            return separators.Riffle(values);
        }
    }
}
