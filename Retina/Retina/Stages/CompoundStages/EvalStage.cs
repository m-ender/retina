using Retina.Configuration;
using Retina.Replace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class EvalStage : Stage
    {
        public Stage ChildStage { get; set; }
        private History History;

        public EvalStage(Config config, History history, Stage childStage)
            : base(config)
        {
            History = history;
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            string replacement = Config.StringParam ?? "$&";
            
            var replacer = new Replacer(replacement, History);
            
            // A bit awkward, but we need to set up a match against the whole input
            // along with the resulting separators and everything, because that's
            // what the Replacer eats.
            var regex = new Regex(@"\A(?s:.*)\z");
            var matches = new List<MatchContext>();
            matches.Add(new MatchContext(regex.Match(input), regex, replacer));
            var separators = new List<MatchContext>();
            var startRegex = new Regex(@"\A");
            separators.Add(new MatchContext(startRegex.Match(input), startRegex, replacer));
            var endRegex = new Regex(@"\z");
            separators.Add(new MatchContext(endRegex.Match(input), endRegex, replacer));

            string evalInput = replacer.Process(input, matches, separators, 0);

            string source = ChildStage.Execute(input, output);

            var interpreter = new Interpreter(source.Split(new[] { '\n' }).Select(line => line.Replace('¶', '\n')).ToList());
            
            var evalOutput = new StringWriter();
            interpreter.Execute(evalInput, evalOutput);

            return evalOutput.ToString();
        }
    }
}