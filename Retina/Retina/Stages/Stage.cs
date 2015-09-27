using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public abstract class Stage
    {
        public bool? Silent { get; set; }

        public Stage() { }

        public string Execute(string input)
        {
            StringBuilder builder = Process(input);

            string result = builder.ToString();

            if (!(Silent ?? true))
                Console.WriteLine(result);

            return result;
        }

        abstract protected StringBuilder Process(string input);
    }
}
