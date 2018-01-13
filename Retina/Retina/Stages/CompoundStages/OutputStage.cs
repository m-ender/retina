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
                Print(input, output);

                return ChildStage.Execute(input, output);
            }
            else
            {
                string result = ChildStage.Execute(input, output);

                if (!Config.PrintOnlyIfChanged || input != result)
                    Print(result, output);

                return result;
            }
        }

        private void Print(string value, TextWriter output)
        {
            if (Config.Random && Random.RNG.Next(2) > 0)
                return;
            
            output.Write(new string(
                value.Where((_, i) => Config.GetLimit(0).IsInRange(i, value.Length)).ToArray()
            ));
            output.Write(Config.StringParam);
        }
    }
}
