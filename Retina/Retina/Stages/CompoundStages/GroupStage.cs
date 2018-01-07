using Retina.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class GroupStage : Stage
    {
        public List<Stage> Stages { get; set; }

        public GroupStage(Config config, List<Stage> stages) 
            : base(config)
        {
            Stages = stages;
        }

        public override string Execute(string input, TextWriter output)
        {
            string result = input;

            if (Config.Random)
            {
                result = Stages[Random.RNG.Next(Stages.Count)].Execute(result, output);
            }
            else
            {
                foreach (var stage in Stages)
                    result = stage.Execute(result, output);
            }

            return result;
        }
    }
}
