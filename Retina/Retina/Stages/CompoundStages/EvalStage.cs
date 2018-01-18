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

        private Replacer Replacer;

        public EvalStage(Config config, History history, Stage childStage)
            : base(config)
        {
            History = history;
            ChildStage = childStage;

            string replacement = Config.StringParam ?? "$&";
            Replacer = new Replacer(replacement, History);
        }

        public override string Execute(string input, TextWriter output)
        {
            // A bit awkward, but we need to set up a match against the whole input
            // along with the resulting separators and everything, because that's
            // what the Replacer eats.
            var regex = new Regex(@"\A(?s:.*)\z");
            var matches = new List<MatchContext>();
            matches.Add(new MatchContext(regex.Match(input), regex, Replacer));
            var separators = new List<MatchContext>();
            var startRegex = new Regex(@"\A");
            separators.Add(new MatchContext(startRegex.Match(input), startRegex, Replacer));
            var endRegex = new Regex(@"\z");
            separators.Add(new MatchContext(endRegex.Match(input), endRegex, Replacer));

            string evalInput = Replacer.Process(input, matches, separators, 0);

            string source = ChildStage.Execute(input, output);

            if (Config.Reverse)
            {
                string temp = source;
                source = evalInput;
                evalInput = temp;
            }

            var interpreter = new Interpreter(source.Split(new[] { '\n' }).Select(line => line.Replace('¶', '\n')).ToList());
            
            var evalOutput = new StringWriter();
            interpreter.Execute(evalInput, evalOutput);

            return evalOutput.ToString();
        }
    }
}