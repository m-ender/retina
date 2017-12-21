using Retina.Configuration;
using Retina.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    class GrepStage : AtomicStage
    {
        public GrepStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Reverse?
            // - Random?
            // - Maybe limit on final lines.
            // - Maybe an option to use a different line separator.

            var values = Matches.Select(s => s.Replacement);

            var separators = Separators.Select(s => s.Match.Value).ToList();

            // Effectively surround the input with a pair of linefeeds, so that all
            // lines are surrounded by linefeeds on both ends.
            separators[0] = "\n" + separators[0];
            separators[separators.Count - 1] += "\n";

            // Any line that appears completely in a separator did not contain a match, so we remove it.
            var greppedSeparators = separators.Select(s => new Regex("\n.*(?=\n)").Replace(s, ""));

            var result = greppedSeparators.Riffle(values);

            // Discard the linefeeds we inserted earlier.
            return result.Substring(1, result.Length - 2);
        }
    }
}
