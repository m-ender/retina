using Retina.Configuration;
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

        public DryRunStage(Config config, Stage childStage)
            : base(config)
        {
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            ChildStage.Execute(input, output);

            return input;
        }
    }
}