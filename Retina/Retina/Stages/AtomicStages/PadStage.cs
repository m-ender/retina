using System.Collections.Generic;
using System.Linq;
using System.IO;
using Retina.Configuration;
using Retina.Extensions;
using System;

namespace Retina.Stages
{
    public class PadStage : AtomicStage
    {
        public PadStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            // TODO:
            // - Random option
            
            var separators = Separators.Select(s => s.Match.Value);

            int paddingWidth = Matches.Select(m => m.Replacement.Length).Aggregate(Math.Max);

            string padString;
            if (!Config.Reverse)
                padString = new string(Enumerable.Range(0, paddingWidth).Select(
                    i => Config.PadString[i % Config.PadString.Length]
                ).ToArray());
            else
                padString = new string(Enumerable.Range(0, paddingWidth).Reverse().Select(
                    i => Config.PadString[Config.PadString.Length - 1 - i % Config.PadString.Length]
                ).ToArray());

            var paddedMatches = Matches.Select(m =>
            {
                string val = m.Match.Value;
                if (!Config.Reverse)
                    return val + padString.Remove(0, Math.Min(val.Length, padString.Length));
                else
                    return padString.Substring(0, Math.Max(0, padString.Length - val.Length)) + val;
            });
            
            return separators.Riffle(paddedMatches);
        }
    }
}
