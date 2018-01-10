using System;
using System.Collections.Generic;

namespace Retina.Replace.Nodes
{
    public class Length : Node
    {
        public Node Child { get; set; }

        public Length(Node child)
        {
            Child = child;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Child.GetLength(input, matches, separators, index).ToString();
        }

        public override int GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Child.GetLength(input, matches, separators, index).ToString().Length;
        }
    }
}
