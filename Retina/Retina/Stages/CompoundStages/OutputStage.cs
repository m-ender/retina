using Retina.Configuration;
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

        public OutputStage(Config config, Stage childStage) 
            : base(config)
        {
            ChildStage = childStage;
        }

        public override string Execute(string input, TextWriter output)
        {
            if (Config.PrePrint)
            {
                output.Write(input);

                if (Config.TrailingLinefeed)
                    output.Write("\n");

                return ChildStage.Execute(input, output);
            }
            else
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
}
