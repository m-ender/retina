using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Replace.Nodes
{
    public class Repetition : Node
    {
        public Node Count { get; set; }
        public Node Value { get; set;}

        public Repetition(Node count, Node value)
        {
            Count = count;
            Value = value;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            string source = Count.GetString(input, matches, separators, index);
            var countPattern = new Regex(@"\d+");
            var countMatch = countPattern.Match(source);
            int count;
            count = countMatch.Success ? int.Parse(countMatch.Value) : 0;

            string value = Value.GetString(input, matches, separators, index);

            return String.Concat(Enumerable.Repeat(value, count));
        }

        public override int GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            string source = Count.GetString(input, matches, separators, index);
            var countPattern = new Regex(@"\d+");
            var countMatch = countPattern.Match(source);
            int count;
            count = countMatch.Success ? int.Parse(countMatch.Value) : 0;

            int valueLength = Value.GetLength(input, matches, separators, index);

            return count * valueLength;
        }
    }
}
