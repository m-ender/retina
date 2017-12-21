using Retina.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Retina.Stages
{
    class MatchStage : AtomicStage
    {
        public MatchStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Potential further limits (on characters, I suppose)
            // - Reverse option
            // - Random option
            return Config.FormatAsList(Matches.Select(m => m.Replacement));
        }
    }
}
