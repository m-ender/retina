using System.Collections.Generic;

namespace Retina.Replace.Nodes
{
    public class LowerCaseAll : Node
    {
        public Node Child { get; set; }

        public LowerCaseAll(Node child)
        {
            Child = child;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Child.GetString(input, matches, separators, index).ToLowerInvariant();
        }

        public override int GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return Child.GetLength(input, matches, separators, index);
        }
    }
}
