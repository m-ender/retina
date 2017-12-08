using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class DryRunStage : Stage
    {
        public Stage ChildStage { get; set; }

        public DryRunStage(Configuration config, Stage childStage)
            : base(config)
        {
            ChildStage = childStage;
        }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            ChildStage.Execute(input, output);

            return new StringBuilder(input);
        }
    }
}