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
            // TODO:
            // - Potential further limits (on characters, I suppose)
            // - Random option
            
            var separators = Separators.Select(s => s.Match.Value);
            var matchReplacements = Matches.Select(s => s.Replacement);

            if (Config.Reverse)
                matchReplacements = matchReplacements.Reverse();

            return separators.Riffle(matchReplacements);
        }
    }
}
