using Retina.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class ConstantStage : Stage
    {
        private string Result { get; set; }

        public ConstantStage(Config config, string result)
            : base(config)
        {
            Result = result;
        }

        public override string Execute(string input, TextWriter output)
        {
            Regex conditionalRegex;

            if (Config.RegexParam != null)
                conditionalRegex = Config.RegexParam;
            else if (Config.StringParam != null)
                conditionalRegex = new Regex(Regex.Escape(Config.StringParam));
            else
                conditionalRegex = new Regex("");
            
            return conditionalRegex.Match(input).Success ? Result : input;
        }
    }
}