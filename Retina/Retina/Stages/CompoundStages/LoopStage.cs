using Retina.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class LoopStage : Stage
    {
        public Stage ChildStage { get; set; }

        public LoopStage(Config config, Stage childStage)
            : base(config)
        {
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            string result = input;
            string lastResult;

            do
            {
                lastResult = result;
                result = ChildStage.Execute(lastResult, output).ToString();
            } while (lastResult != result);

            return result;
        }
    }
}