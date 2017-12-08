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

        public LoopStage(Configuration config, Stage childStage)
            : base(config)
        {
            ChildStage = childStage;
        }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            string result = input;
            string lastResult;

            do
            {
                lastResult = result;
                result = ChildStage.Execute(lastResult, output).ToString();
            } while (lastResult != result);

            return new StringBuilder(result);
        }
    }
}