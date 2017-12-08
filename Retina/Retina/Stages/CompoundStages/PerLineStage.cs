using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class PerLineStage : Stage
    {
        public Stage ChildStage { get; set; }

        public PerLineStage(Configuration config, Stage childStage)
            : base(config)
        {
            ChildStage = childStage;
        }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            string[] lines = input.Split(new[] { '\n' });
            IEnumerable<string> resultLines = lines.Select(x => ChildStage.Execute(x, output));
            string result = String.Join("\n", resultLines);
            
            return new StringBuilder(result);
        }
    }
}