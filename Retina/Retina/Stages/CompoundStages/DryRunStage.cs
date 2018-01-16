using Retina.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class DryRunStage : Stage
    {
        public Stage ChildStage { get; set; }
        private History History;
        private int HistoryIndex;

        public DryRunStage(Config config, History history, Stage childStage)
            : base(config)
        {
            History = history;
            if (Config.RegexParam != null || Config.StringParam != null)
                HistoryIndex = History.RegisterStage();
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            string result = ChildStage.Execute(input, output);

            if (Config.RegexParam != null || Config.StringParam != null)
            {
                var regex = Config.RegexParam ?? new Regex(Regex.Escape(Config.StringParam));

                if (!(regex.Match(result).Success ^ Config.Reverse))
                    result = input;

                History.RegisterResult(HistoryIndex, result);

                return result;
            }
            else
                return input;
        }
    }
}