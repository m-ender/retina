using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina
{
    class Options
    {
        public RegexOptions RegexOptions { get; set; }

        public Modes Mode { get; set; }

        // Options for Match mode
        public bool Overlapping { get; set; }
        public bool PrintMatches { get; set; }

        // Options for Split mode
        public bool OmitEmpty { get; set; }

        // Options for Replace mode
        public bool Loop { get; set; }
        public bool Trace { get; set; }

        public Options()
        {
            RegexOptions = RegexOptions.None;
            Mode = Modes.Match;

            Overlapping = false;
            PrintMatches = false;

            OmitEmpty = false;

            Loop = false;
            Trace = false;
        }

    }
}
