using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Retina.Replace.Nodes
{
    public class DynamicElement : Node
    {
        public Node Child { get; set; }
        private bool CyclicMatches;

        private History History;

        public DynamicElement(Node child, History history, bool cyclicMatches)
        {
            Child = child;
            History = history;
            CyclicMatches = cyclicMatches;
        }

        public DynamicElement(string child, History history, bool cyclicMatches)
        {
            Child = new Literal(child);
            History = history;
            CyclicMatches = cyclicMatches;
        }

        public override string GetString(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return ResolveChild(input, matches, separators, index);
        }

        public override int GetLength(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            return ResolveChild(input, matches, separators, index).Length;
        }

        private string ResolveChild(string input, List<MatchContext> matches, List<MatchContext> separators, int index)
        {
            string childValue = Child.GetString(input, matches, separators, index);

            var elementParser = new Regex(@"
              \A                    # Use \A and \z to ensure that the pattern covers the entire string.
              (?<length>[.])?       # An optional length modifier is always available.
              (
                (?<adjacent>        # Pull the value from an adjacent separator or match.
                  [<>[\]]
                )?
                (
                  (?<modifier>      # Capture count or random capture.
                    [#?]
                  )?
                  (
                    (?<entireMatch> # $& and $0 refer to the entire match.
                      &|0+
                    )
                  |
                    (?<group>
                      (?<number>    # n are numbered groups.
                        \d+
                      )
                    |
                      (?<name>      # other words are named groups.
                        [^\W\d]\w*
                      )
                    )
                  )
                |
                  (?<lineOnly>%)?   # Stop at the nearest linefeed.
                  (?<context>       # $`, $' and $_ are context elements.
                    [`'=""]
                  )
                )
              |
                (?<history>         # $-n, $+n are history elements.
                  [-+]
                  (?<index>\d+)
                )
              )
              \z
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

            Match parserMatch = elementParser.Match(childValue);

            if (!parserMatch.Success)
                return "";

            bool length = parserMatch.Groups["length"].Success;

            char? adjacent = null;
            if (parserMatch.Groups["adjacent"].Success)
                adjacent = parserMatch.Groups["adjacent"].Value[0];

            char? modifier = null;
            if (parserMatch.Groups["modifier"].Success)
                modifier = parserMatch.Groups["modifier"].Value[0];

            MatchContext match;
            switch (adjacent)
            {
            case '<':
                match = separators[index];
                break;
            case '>':
                match = separators[index+1];
                break;
            case '[':
                if (CyclicMatches)
                    match = matches[(index - 1 + matches.Count) % matches.Count];
                else if (index == 0)
                    return "";
                else
                    match = matches[index - 1];
                break;
            case ']':
                if (CyclicMatches)
                    match = matches[(index + 1) % matches.Count];
                else if (index == matches.Count - 1)
                    return "";
                else
                    match = matches[index + 1];
                break;
            default:
                match = matches[index];
                break;
            }

            string value;

            if (parserMatch.Groups["entireMatch"].Success)
            {
                switch (modifier)
                {
                case '#':
                    // Subtract 1 to account for group 0.
                    value = (match.Regex.GetGroupNumbers().Count(i => match.Match.Groups[i].Success) - 1).ToString();
                    break;
                case '?':
                    int[] groups = match.Regex.GetGroupNumbers();
                    // Offset by 1 to account for group 0.
                    int randomGroup = groups[Random.RNG.Next(groups.Length - 1) + 1];
                    value = match.Match.Groups[randomGroup].Value;
                    break;
                default:
                    value = match.Match.Value;
                    break;
                }
            }
            else if (parserMatch.Groups["group"].Success)
            {
                Group group;
                if (parserMatch.Groups["number"].Success)
                {
                    int number = int.Parse(parserMatch.Groups["number"].Value);
                    int[] groups = match.Regex.GetGroupNumbers();
                    if (!groups.Contains(number))
                        return "";

                    group = match.Match.Groups[number];
                }
                else if (parserMatch.Groups["name"].Success)
                {
                    string name = parserMatch.Groups["name"].Value;
                    string[] groups = match.Regex.GetGroupNames();
                    if (!groups.Contains(name))
                        return "";

                    group = match.Match.Groups[name];
                }
                else
                    throw new Exception("This shouldn't happen!");
                
                switch (modifier)
                {
                case '#':
                    value = group.Captures.Count.ToString();
                    break;
                case '?':
                    // Early return so that we don't get a 0 if length is also used.
                    if (!group.Success)
                        return "";

                    value = group.Captures[Random.RNG.Next(group.Captures.Count)].Value;
                    break;
                default:
                    // Early return so that we don't get a 0 if length is also used.
                    if (!group.Success)
                        return "";

                    value = group.Value;
                    break;
                }
            }
            else if (parserMatch.Groups["context"].Success)
            {
                bool lineOnly = parserMatch.Groups["lineOnly"].Success;

                // We need these for at least two different cases, so we just compute
                // them anyway. We could limit this based on which cases we have, but
                // for now I prefer the cleaner code over the faster one.
                string prefix = input.Substring(0, match.Match.Index);
                string suffix = input.Substring(match.Match.Index + match.Match.Length);

                if (lineOnly)
                {
                    int start = prefix.LastIndexOf('\n') + 1;
                    prefix = prefix.Substring(start);

                    int end = suffix.IndexOf('\n');
                    if (end >= 0) suffix = suffix.Substring(0, end);
                }

                switch (parserMatch.Groups["context"].Value[0])
                {
                case '`':
                    value = prefix;
                    break;
                case '\'':
                    value = suffix;
                    break;
                case '=':
                    value = input;

                    if (lineOnly)
                    {
                        int start = value.LastIndexOf('\n', match.Match.Index) + 1;
                        int end = value.IndexOf('\n', match.Match.Index + match.Match.Length);
                        if (end == -1) end = value.Length;
                        value = value.Substring(start, end - start);
                    }

                    break;
                case '"':
                    value = suffix + "\n" + prefix;
                    break;
                default:
                    throw new Exception("Unknown context element encountered.");
                }   
            }
            else if (parserMatch.Groups["history"].Success)
            {
                // TODO: Think about allowing $- and $+.

                int number = int.Parse(parserMatch.Groups["index"].Value);
                switch (parserMatch.Groups["history"].Value[0])
                {
                case '-':
                    value = History.GetMostRecentResult(number);
                    if (value == null)
                        return "";
                    break;
                case '+':
                    value = History.GetStageResult(number);
                    if (value == null)
                        return "";
                    break;
                default:
                    throw new Exception("Unknown history element encountered.");
                }
            }
            else
            {
                throw new Exception("This shouldn't happen.");
            }

            return length ? value.Length.ToString() : value;
        }
    }
}
