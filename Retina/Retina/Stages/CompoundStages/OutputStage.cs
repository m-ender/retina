using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class OutputStage : Stage
    {
        public Stage ChildStage { get; set; }

        public OutputStage(Configuration config, Stage childStage) 
            : base(config)
        {
            ChildStage = childStage;
        }

        protected override StringBuilder Process(string input, TextWriter output)
        {
            string result = ChildStage.Execute(input, output);

            if (!Config.PrintOnlyIfChanged || input != result)
            {
                output.Write(result);
                if (Config.TrailingLinefeed)
                    output.Write("\n");
            }

            return new StringBuilder(result);
        }
    }
}
