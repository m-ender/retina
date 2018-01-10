using System;
using System.Collections.Generic;

namespace Retina.Replace.Nodes
{
    public class Reverse : Node
    {
        public Node Child { get; set; }

        public Reverse(Node child)
        {
            Child = child;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            char[] charArray = Child.GetString(input, matches, separators, index).ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public override int GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Child.GetLength(input, matches, separators, index);
        }
    }
}
