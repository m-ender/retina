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
    class GroupStage : Stage
    {
        public List<Stage> Stages { get; set; }
        private History History;
        private int HistoryIndex;

        public GroupStage(Config config, History history, List<Stage> stages) 
            : base(config)
        {
            History = history;
            HistoryIndex = History.RegisterStage();
            Stages = stages;
        }

        public override string Execute(string input, TextWriter output)
        {
            string result = input;

            if (Config.Random)
            {
                result = Stages[Random.RNG.Next(Stages.Count)].Execute(result, output);
            }
            else if (Config.RegexParam != null)
            {
                if (Config.RegexParam.Match(input).Success ^ Config.Reverse)
                    result = Stages[0].Execute(result, output);
                else
                    for (int i = 1; i < Stages.Count; ++i)
                        result = Stages[i].Execute(result, output);
            }
            else
            {
                foreach (var stage in Stages)
                    result = stage.Execute(result, output);
            }

            History.RegisterResult(HistoryIndex, result);

            return result;
        }
    }
}
