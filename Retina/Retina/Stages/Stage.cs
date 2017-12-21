using Retina.Configuration;
using System.IO;

namespace Retina.Stages
{
    public abstract class Stage
    {
        public Config Config { get; set; }

        public Stage(Config config)
        {
            Config = config;
        }

        abstract public string Execute(string input, TextWriter output);
    }
}
