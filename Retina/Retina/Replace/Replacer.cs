using Retina.Replace.Nodes;
using System.Collections.Generic;

namespace Retina.Replace
{
    public class Replacer
    {
        private Node ReplaceTree { get; set; }

        public Replacer(string replacement)
        {
            ReplaceTree = new Parser().ParseReplacement(replacement);
        }

        public Replacer(string replacement, bool cyclicMatches)
        {
            ReplaceTree = new Parser(cyclicMatches).ParseReplacement(replacement);
        }

        public string Process(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return ReplaceTree.GetString(input, matches, separators, index);
        }
    }
}
