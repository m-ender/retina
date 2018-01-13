using Retina.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class ConditionalStage : Stage
    {
        public Stage ChildStage { get; set; }

        public ConditionalStage(Config config, Stage childStage)
            : base(config)
        {
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            if (Config.Random)
            {
                return Random.RNG.Next(2) > 0 ? ChildStage.Execute(input, output) : input;
            }

            Regex regex;

            if (Config.RegexParam != null || Config.StringParam != null)
                regex = Config.RegexParam ?? new Regex(Regex.Escape(Config.StringParam));
            else
                regex = new Regex("");
            
            if (regex.Match(input).Success ^ Config.Reverse)
                return ChildStage.Execute(input, output);

            return input;
        }
    }
}