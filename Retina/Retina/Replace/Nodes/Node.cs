using System.Collections.Generic;

namespace Retina.Replace.Nodes
{
    public abstract class Node
    {
        abstract public string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index);
        abstract public int GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index);
    }
}
