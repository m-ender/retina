using Retina.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class ConstantStage : Stage
    {
        private string Result { get; set; }
        private History History;
        private int HistoryIndex;
        private bool RegisterWithHistory;

        public ConstantStage(Config config, History history, bool registerByDefault, string result)
            : base(config)
        {
            Result = result;
            History = history;
            RegisterWithHistory = registerByDefault ^ Config.RegisterToggle;
            if (RegisterWithHistory)
                HistoryIndex = History.RegisterStage();
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
            
            string result = (conditionalRegex.Match(input).Success ^ Config.Reverse) ? Result : input;

            if (RegisterWithHistory)
                History.RegisterResult(HistoryIndex, result);

            return result;
        }
    }
}