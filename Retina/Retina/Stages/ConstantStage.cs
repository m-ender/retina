using Retina.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public class ConstantStage : Stage
    {
        private string Result { get; set; }

        public ConstantStage(Config config, string result)
            : base(config)
        {
            Result = result;
        }

        public override string Execute(string input, TextWriter output)
        {
            return Result;
        }
    }
}