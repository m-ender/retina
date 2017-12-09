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

        public override string Execute(string input, TextWriter output)
        {
            string result = ChildStage.Execute(input, output);

            if (!Config.PrintOnlyIfChanged || input != result)
            {
                output.Write(result);
                if (Config.TrailingLinefeed)
                    output.Write("\n");
            }

            return result;
        }
    }
}
