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
            // TODO:
            // - Random option
            var values = Matches.Select(m => {
                int i = 0;
                return new string(m.Replacement.Where(_ => Config.GetLimit(1).IsInRange(i++, m.Replacement.Length)).ToArray());
            });
            
            if (Config.Reverse)
                values = values.Reverse();

            return Config.FormatAsList(values);
        }
    }
}
