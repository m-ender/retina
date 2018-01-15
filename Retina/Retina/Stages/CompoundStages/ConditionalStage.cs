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
        private History History;
        private int HistoryIndex;

        public ConditionalStage(Config config, History history, Stage childStage)
            : base(config)
        {
            History = history;
            HistoryIndex = History.RegisterStage();
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            string result;

            if (Config.Random)
            {
                result = Random.RNG.Next(2) > 0 ? ChildStage.Execute(input, output) : input;
            }
            else
            {
                Regex regex;

                if (Config.RegexParam != null || Config.StringParam != null)
                    regex = Config.RegexParam ?? new Regex(Regex.Escape(Config.StringParam));
                else
                    regex = new Regex("");

                if (regex.Match(input).Success ^ Config.Reverse)
                    result = ChildStage.Execute(input, output);
                else
                    result = input;
            }

            History.RegisterResult(HistoryIndex, result);
            return result;
        }
    }
}