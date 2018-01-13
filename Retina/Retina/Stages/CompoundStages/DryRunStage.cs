using Retina.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class DryRunStage : Stage
    {
        public Stage ChildStage { get; set; }

        public DryRunStage(Config config, Stage childStage)
            : base(config)
        {
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            string result = ChildStage.Execute(input, output);
            
            if (Config.RegexParam != null || Config.StringParam != null)
            {
                var regex = Config.RegexParam ?? new Regex(Regex.Escape(Config.StringParam));

                if (regex.Match(result).Success ^ Config.Reverse)
                    return result;
                else
                    return input;
            }

            return input;
        }
    }
}