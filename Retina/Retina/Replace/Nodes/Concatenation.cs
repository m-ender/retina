using System;
using System.Collections.Generic;
using System.Linq;

namespace Retina.Replace.Nodes
{
    public class Concatenation : Node
    {
        public List<Node> Children { get; set; }

        public Concatenation(List<Node> children)
        {
            Children = children;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return String.Concat(Children.Select(c => c.GetString(input, matches, separators, index)));
        }

        public override int GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Children.Select(c => c.GetLength(input, matches, separators, index)).Sum();
        }
    }
}
