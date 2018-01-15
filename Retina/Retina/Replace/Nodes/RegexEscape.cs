using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Retina.Replace.Nodes
{
    public class RegexEscape : Node
    {
        public Node Child { get; set; }

        public RegexEscape(Node child)
        {
            Child = child;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            string value = Regex.Escape(Child.GetString(input, matches, separators, index));
            // We also escape slashes, so the result can be used in a slash-delimited regex.
            return value.Replace("/", "\\/");
        }

        public override BigInteger GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            string value = Regex.Escape(Child.GetString(input, matches, separators, index));
            // We also escape slashes, so the result can be used in a slash-delimited regex.
            return value.Replace("/", "\\/").Length;
        }
    }
}
