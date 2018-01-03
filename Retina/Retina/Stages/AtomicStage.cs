using Retina.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Retina.Stages
{
    public abstract class AtomicStage : Stage
    {
        // These are independent of a particular run of this stage.
        // Patterns might change for stages with Overlaps.All, but otherwise
        // these only depend on the parsed source code.
        public List<string> RegexSources { get; set; }
        public List<Regex> Regices { get; set; }
        public List<string> SubstitutionSources { get; set; }
        public string SeparatorSubstitutionSource { get; set; }

        // These get recomputed each time the stage is executed.
        public List<MatchContext> Matches { get; set; }
        public List<MatchContext> Separators { get; set; }
        private int PatternIndex;

        protected AtomicStage(Config config) : base(config) { }

        public AtomicStage(Config config, List<string> regexSources, List<string> substitutionSources, string separatorSubstitutionSource) 
            : this(config)
        {
            RegexSources = regexSources;
            SubstitutionSources = substitutionSources;
            SeparatorSubstitutionSource = separatorSubstitutionSource;
        }

        public override string Execute(string input, TextWriter output)
        {
            Matches = new List<MatchContext>();
            Separators = new List<MatchContext>();

            if (Config.InputAsRegex)
            {
                // Swap input with first regex. There should be only one regex anyway.
                string temp = input;
                input = RegexSources[0];
                RegexSources[0] = temp;
            }

            switch (Config.Anchoring)
            {
            case Anchoring.String:
                RegexSources = RegexSources.ConvertAll(s => WrapRegex(@"\A", s, @"\z"));
                break;
            case Anchoring.Line:
                RegexSources = RegexSources.ConvertAll(s => WrapRegex(@"(?m:^)", s, @"(?m:$)"));
                break;
            }

            Regices = RegexSources.ConvertAll(source => new Regex(source, Config.RegexOptions));

            FindAllMatches(input);

            LimitMatches();

            if (Config.RegexOptions.HasFlag(RegexOptions.RightToLeft))
                Matches.Reverse();

            FillSeparators(input);

            if (Config.InvertMatches)
                InvertMatches(input);


            // Apply the corresponding substitution to each match.
            foreach (var matchContext in Matches)
                matchContext.Replacement = matchContext.Replacer.Process(input, matchContext.Match);

            return Process(input, output);
        }

        // Fills MatchContexts, taking into account Config.Overlaps and Config.Unique, as well as multi-pattern setups.
        private void FindAllMatches(string input)
        {
            PatternIndex = 0;

            switch (Config.Overlaps)
            {
            case Overlaps.None:
                if (!Config.RegexOptions.HasFlag(RegexOptions.RightToLeft))
                {
                    int startPos = 0;
                    while (startPos <= input.Length)
                    {
                        var matchContext = GetMatch(input, startPos);
                        if (!matchContext.Match.Success) break;
                        RecordMatch(matchContext);
                        startPos = matchContext.Match.Index + matchContext.Match.Length;
                        // Make sure we advance the cursor even if the match
                        // is an empty string.
                        if (matchContext.Match.Length == 0) ++startPos;
                    }
                }
                else
                {
                    int startPos = input.Length;
                    while (startPos >= 0)
                    {
                        var matchContext = GetMatch(input, startPos);
                        if (!matchContext.Match.Success) break;
                        RecordMatch(matchContext);
                        startPos = matchContext.Match.Index;
                        // Make sure we advance the cursor even if the match
                        // is an empty string.
                        if (matchContext.Match.Length == 0) --startPos;
                    }
                }
                break;
            case Overlaps.OverlappingSimple:
                if (!Config.RegexOptions.HasFlag(RegexOptions.RightToLeft))
                {
                    int startPos = 0;

                    while (startPos <= input.Length)
                    {
                        var matchContext = GetMatch(input, startPos);
                        if (!matchContext.Match.Success) break;
                        RecordMatch(matchContext);
                        startPos = matchContext.Match.Index + 1;
                    }
                }
                else
                {
                    int startPos = input.Length;

                    while (startPos >= 0)
                    {
                        var matchContext = GetMatch(input, startPos);
                        if (!matchContext.Match.Success) break;
                        RecordMatch(matchContext);
                        startPos = matchContext.Match.Index + matchContext.Match.Length - 1;
                    }
                }
                break;
            case Overlaps.OverlappingAll:
                if (!Config.RegexOptions.HasFlag(RegexOptions.RightToLeft))
                {
                    for (int startPos = 0; startPos <= input.Length; ++startPos)
                    {
                        for (int length = 0; length <= input.Length - startPos; ++length)
                        {
                            var matchContext = GetMatch(input, startPos, length);
                            if (matchContext.Match.Success)
                                RecordMatch(matchContext);
                        }
                    }
                }
                else
                {
                    for (int startPos = input.Length; startPos >= 0; --startPos)
                    {
                        for (int length = 0; length <= startPos; ++length)
                        {
                            var matchContext = GetMatch(input, startPos, length);
                            if (matchContext.Match.Success)
                                RecordMatch(matchContext);
                        }
                    }
                }
                break;
            default:
                throw new NotImplementedException();
            }
        }

        // Apply the first limit that is common to all stage types.
        private void LimitMatches()
        {
            Limit matchLimit = Config.GetLimit(0);
            int matchCount = Matches.Count;
            for (int i = matchCount-1; i >= 0; --i)
                if (!matchLimit.IsInRange(i, matchCount))
                    Matches.RemoveAt(i);
        }

        // Generate matches for the segments between matches.
        // When matches overlap, insert an empty match at the beginning of the second match.
        private void FillSeparators(string input)
        {
            Regex sepRegex;
            Match sepMatch;
            int lastEnd = 0;
            foreach (MatchContext m in Matches)
            {
                sepRegex = new Regex(
                    String.Format(@"\G.{{{0}}}", Math.Max(m.Match.Index - lastEnd, 0)),
                    RegexOptions.Singleline
                );
                sepMatch = sepRegex.Match(input, lastEnd);

                Separators.Add(new MatchContext(sepMatch, sepRegex, SeparatorSubstitutionSource));
                lastEnd = m.Match.Index + m.Match.Length;
            }

            // Add the final segment, after the last match.

            sepRegex = new Regex(
                @"\G.*",
                RegexOptions.Singleline
            );
            sepMatch = sepRegex.Match(input, lastEnd);

            Separators.Add(new MatchContext(sepMatch, sepRegex, SeparatorSubstitutionSource));
        }

        // Swap the Separators and Matches such that it seems that the Separators were actually matched.
        private void InvertMatches(string input)
        {
            var temp = new List<MatchContext>();
            var regex = new Regex(@"\A");
            temp.Add(new MatchContext(regex.Match(input), regex, "$&"));
            temp.AddRange(Matches);
            regex = new Regex(@"\z");
            temp.Add(new MatchContext(regex.Match(input), regex, "$&"));

            Matches = Separators;
            Separators = temp;
        }

        // Returns a pair of match and the corresponding substitution string.
        private MatchContext GetMatch(string input, int startPos, int length = -1)
        {
            if (Regices.Count > 1 && Config.Greedy)
            {
                // Try all matches, pick the one with the earliest match position.

                List<Regex> patterns;

                if (length < 0)
                {
                    patterns = Regices;
                }
                else
                {
                    if (!Config.RegexOptions.HasFlag(RegexOptions.RightToLeft))
                        patterns = RegexSources.ConvertAll(s => new Regex(WrapRegex(
                            String.Format("\\G(?=(?s:.{{{0}}}(?<_suffix>.*)))", length),
                            s,
                            @"(?=\k<_suffix>(?<-_suffix>)\z)"
                        ), Config.RegexOptions));
                    else
                        patterns = RegexSources.ConvertAll(s => new Regex(WrapRegex(
                            @"(?<=\A(?<-_suffix>)\k<_suffix>)",
                            s,
                            String.Format("(?<=(?s:(?<_suffix>.*).{{{0}}}))\\G", length)
                        ), Config.RegexOptions));
                }

                var matches = new List<MatchContext>();

                
                for (int i = 0; i < patterns.Count; ++i)
                    matches.Add(new MatchContext(
                        patterns[i].Match(input, startPos),
                        patterns[i],
                        SubstitutionSources[i]
                    ));

                // For RTL matching, we have to pick the right-most match.
                if (!Config.RegexOptions.HasFlag(RegexOptions.RightToLeft))
                    return matches.Aggregate((curMin, m) =>
                        (m.Match.Index < curMin.Match.Index) ? m : curMin
                    );
                else
                    return matches.Aggregate((curMin, m) =>
                        (m.Match.Index + m.Match.Length > curMin.Match.Index + curMin.Match.Length) ? m : curMin
                    );
            }
            else
            {
                // Cycle through patterns

                string substitution = SubstitutionSources[PatternIndex];

                if (length < 0)
                {
                    // No length constraint, use regular pattern
                    Regex pattern = Regices[PatternIndex];
                    
                    Match m = pattern.Match(input, startPos);

                    if (m.Success)
                    {
                        ++PatternIndex;
                        PatternIndex %= Regices.Count;
                    }

                    return new MatchContext(m, pattern, substitution);
                }
                else
                {
                    // Length is constrained, create length-specific pattern.
                    // This also constrains the starting position of the match
                    // exactly instead of giving a lower bound.
                    string patternString = RegexSources[PatternIndex];
                    
                    // TODO: Compile the pattern first to find a guaranteed unused group name.
                    // However, this technique still messes with group numbers of explicitly
                    // named groups.
                    if (!Config.RegexOptions.HasFlag(RegexOptions.RightToLeft))
                        patternString = WrapRegex(
                            String.Format("\\G(?=(?s:.{{{0}}}(?<_suffix>.*)))", length),
                            patternString,
                            @"(?=\k<_suffix>(?<-_suffix>)\z)"
                        );
                    else
                        patternString = WrapRegex(
                            @"(?<=\A(?<-_suffix>)\k<_suffix>)",
                            patternString,
                            String.Format("(?<=(?s:(?<_suffix>.*).{{{0}}}))\\G", length)
                        );

                    Regex pattern = new Regex(patternString, Config.RegexOptions);

                    Match m = pattern.Match(input, startPos);

                    if (m.Success)
                    {
                        ++PatternIndex;
                        PatternIndex %= Regices.Count;
                    }

                    return new MatchContext(m, pattern, substitution);
                }
            }
        }

        private void RecordMatch (MatchContext matchContext) {
            switch (Config.UniqueMatches)
            {
            case UniqueMatches.Off:
                Matches.Add(matchContext);
                break;
            case UniqueMatches.KeepFirst:
                if (!Matches.Any(m => m.Match.Value == matchContext.Match.Value))
                    Matches.Add(matchContext);
                break;
            case UniqueMatches.KeepLast:
                Matches.RemoveAll(m => m.Match.Value == matchContext.Match.Value);
                Matches.Add(matchContext);
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private string WrapRegex (string prefix, string pattern, string suffix)
        {
            return String.Format(
                "{0}(?:{1}{2}){3}",
                prefix,
                pattern,
                Config.RegexOptions.HasFlag(RegexOptions.IgnorePatternWhitespace) ? "\n" : "",
                suffix
            );
        }


        abstract protected string Process(string input, TextWriter output);
    }
}
