using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Retina.Replace.Nodes
{
    public class TitleCaseAll : Node
    {
        public Node Child { get; set; }

        public TitleCaseAll(Node child)
        {
            Child = child;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            string value = Child.GetString(input, matches, separators, index);

            return new Regex(@"\p{L}+", RegexOptions.IgnoreCase).Replace(value, 
                m => Char.ToUpperInvariant(m.Value[0]) + m.Value.Substring(1).ToLowerInvariant()
            );
        }

        public override BigInteger GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Child.GetLength(input, matches, separators, index);
        }
    }
}
