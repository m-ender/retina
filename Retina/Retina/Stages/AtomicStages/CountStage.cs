using Retina.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Retina.Stages
{
    class CountStage : AtomicStage
    {
        public CountStage(Config config, History history, bool registerByDefault, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, history, registerByDefault, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            return Matches.Count.ToString();
        }
    }
}
