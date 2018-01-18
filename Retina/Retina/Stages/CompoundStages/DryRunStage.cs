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
        private bool RegisterWithHistory;

        public DryRunStage(Config config, History history, bool registerByDefault, Stage childStage)
            : base(config)
        {
            History = history;
            if (Config.RegexParam != null || Config.StringParam != null)
                RegisterWithHistory = registerByDefault ^ Config.RegisterToggle;
            else
                RegisterWithHistory = false;

            if (RegisterWithHistory)
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
            }
            else
                result = input;

            if (RegisterWithHistory)
                History.RegisterResult(HistoryIndex, result);

            return result;
        }
    }
}