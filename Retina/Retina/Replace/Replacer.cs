using Retina.Replace.Nodes;
using System.Collections.Generic;

namespace Retina.Replace
{
    public class Replacer
    {
        private Node ReplaceTree { get; set; }

        public Replacer(string replacement, History history)
        {
            ReplaceTree = new Parser(history).ParseReplacement(replacement);
        }

        public Replacer(string replacement, History history, bool cyclicMatches)
        {
            ReplaceTree = new Parser(history, cyclicMatches).ParseReplacement(replacement);
        }

        public string Process(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return ReplaceTree.GetString(input, matches, separators, index);
        }
    }
}
