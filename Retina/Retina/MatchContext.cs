using Retina.Replace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina
{
    public class MatchContext
    {
        public Match Match { get; set; }
        public Replacer Replacer { get; set; }
        public string Replacement { get; set; }
        
        public MatchContext(Match match, Regex regex, string substitutionSource)
        {
            Match = match;
            Replacer = new Replacer(regex, substitutionSource);
        }
    }
}
