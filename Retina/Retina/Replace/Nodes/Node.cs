using System.Collections.Generic;
using System.Numerics;

namespace Retina.Replace.Nodes
{
    public abstract class Node
    {
        abstract public string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index);
        abstract public BigInteger GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index);
    }
}
