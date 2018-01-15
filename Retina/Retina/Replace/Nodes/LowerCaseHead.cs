using System;
using System.Collections.Generic;
using System.Numerics;

namespace Retina.Replace.Nodes
{
    public class LowerCaseHead : Node
    {
        public Node Child { get; set; }

        public LowerCaseHead(Node child)
        {
            Child = child;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            string value = Child.GetString(input, matches, separators, index);

            if (value == "")
                return "";
            else
                return Char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public override BigInteger GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Child.GetLength(input, matches, separators, index);
        }
    }
}
