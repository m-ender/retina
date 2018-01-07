using Retina.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Retina.Stages
{
    class ListStage : AtomicStage
    {
        public ListStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            var values = Matches.Select(m => new string(
                m.Replacement.Where((_, i) => Config.GetLimit(1).IsInRange(i, m.Replacement.Length)).ToArray()
            ));
            
            if (Config.Reverse)
                values = values.Reverse();

            return Config.FormatAsList(values);
        }
    }
}
