using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    abstract class Stage
    {
        public Stage() { }

        abstract public string Execute(string input);
    }
}
