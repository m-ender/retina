using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class LoopStage : Stage
    {
        public List<Stage> Stages { get; set; }

        public LoopStage(List<Stage> stages, bool? silent = null)
        {
            if (silent != null)
                Silent = silent;
            Stages = stages;
        }

        public override string Execute(string input)
        {
            string result = input;
            string lastResult;
            do
            {
                lastResult = result;
                foreach (var stage in Stages)
                    result = stage.Execute(result);
            } while (lastResult != result);

            if (!(Silent ?? true))
                Console.WriteLine(result);

            return result;
        }
    }
}
