using System.Collections.Generic;
using System.Linq;
using System.IO;
using Retina.Configuration;
using Retina.Extensions;

namespace Retina.Stages
{
    public class ReplaceStage : AtomicStage
    {
        public ReplaceStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            var separators = Separators.Select(s => s.Match.Value);
            var matchReplacements = Matches.Select(m => new string(
                m.Replacement.Where((_, i) => Config.GetLimit(1).IsInRange(i, m.Replacement.Length)).ToArray()
            ));

            if (Config.Reverse)
                matchReplacements = matchReplacements.Reverse();

            return separators.Riffle(matchReplacements);
        }
    }
}
