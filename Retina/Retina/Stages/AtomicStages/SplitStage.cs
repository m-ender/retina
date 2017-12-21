using Retina.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Retina.Stages
{
    class SplitStage : AtomicStage
    {
        public SplitStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Further limits (on final list)
            // - Reverse option
            // - Random option?
            // - OmitGroups (or rather, include them in the first place)
            // - OmitEmpty
            return Config.FormatAsList(Separators.Select(s => s.Replacement));
        }
    }
}
