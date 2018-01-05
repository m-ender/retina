using Retina.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Retina.Stages
{
    class PositionStage : AtomicStage
    {
        public PositionStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Random option
            var values = Matches.Select(m => (Config.Reverse ? m.Match.Index + m.Match.Length : m.Match.Index).ToString());
            
            return Config.FormatAsList(values);
        }
    }
}
