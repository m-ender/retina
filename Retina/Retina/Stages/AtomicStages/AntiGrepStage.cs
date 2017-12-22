using Retina.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    class AntiGrepStage : AtomicStage
    {
        public AntiGrepStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource) 
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Reverse?
            // - Random?
            // - Maybe limit on final lines.
            // - Maybe an option to use a different line separator.

            var values = Matches.Select(s => s.Replacement);

            List<string> separators = Separators.Select(s => s.Match.Value).ToList();

            // Effectively surround the input with a pair of linefeeds, so that all
            // lines are surrounded by linefeeds on both ends.
            separators[0] = "\n" + separators[0];
            separators[separators.Count - 1] += "\n";

            // Any line that appears at the boundary of a separator was touched by a match,
            // so we remove it.
            // This always discards the leading dummy line, but keeps the trailing dummy line.
            var greppedSeparators = separators.Select(s => new Regex(@"^.*\n|.*\z").Replace(s, ""));

            var result = String.Join("", greppedSeparators);

            // Discard the trailing dummy line unless it's the only one left
            // (a single empty line is not distinguishable from zero empty lines).
            if (result.Length > 0)
                return result.Substring(0, result.Length - 1);
            else
                return "";
        }
    }
}
