using Retina.Replace;
using System.Text.RegularExpressions;

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
