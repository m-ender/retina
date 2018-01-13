using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Retina.Stages;
using Retina.Configuration;

namespace Retina
{
    public class Interpreter
    {
        private Stage StageTree;
        private History History;

        public Interpreter (List<string> sources)
	    {
            History = new History();

            // This stack is used for building the stage tree. The list on top is
            // always a list of leaf nodes that we're currently building. When we
            // reach a group, we'll push a new list onto it, because the next stages
            // will be one level deeper. When we reach the end of a group, we'll
            // wrap the current list in that group, pop the list and then add the
            // group to the end of the now-top list.
            var stageStack = new Stack<List<Stage>>();
            stageStack.Push(new List<Stage>());

            // When we form a group, there are always two parts to the configuration:
            // one in front of the '(' and one in front of the ')'. We use this stack
            // to keep track of the '(' config until we get to the ')' config, where
            // we can merge the two into one.
            var groupConfig = new Stack<Config>();
            
            // Within each stage, we'll also need to keep track of a stack of compound
            // stages that will eventually be wrapped around that stage. This is because
            // the stages wrap from right to left, so we need to apply them in the
            // reverse order they are parsed. However, compounds outside of a '(' will
            // only need to be applied once we get to the ')', so like the '(' config,
            // we need to remember the compounds we've already seen. That's what this
            // messy stack of stacks is for.
            var groupCompoundStack = new Stack<Stack<Tuple<char, Config>>>();

            // A global option that affects whether the whole program will be wrapped
            // in an output stage or not.
            var silent = false;

            int i = 0;
            while (i < sources.Count)
            {
                string pattern = sources[i++];

                Modes mode = i < sources.Count ? Modes.Replace : Modes.Count;
                bool useSubstitution = false;
                int patternCount = 1;

                Config config = new Config();

                // Compound stages will be wrapped around this stage once we're done
                // constructing it. However, they will be process from right to left
                // so we keep track of them on a stack. Each stack element holds the
                // type of compound stage as the character that represents it, together
                // with the configuration that was immediately left of it.
                // Groups will enter this stack at the point of their closing ')'.
                var compoundStack = new Stack<Tuple<char, Config>>();

                // Backticks indicate a leading configuration string.
                if (pattern.Contains("`"))
                {
                    var configTokenizer = new Regex(@"\G(  # Use \G to ensure that the tokens cover the entire string.
                        (?<limit>
                            (?<lneg>\^)?                    # Any limit can be negated with an optional ^.
                            (
                                (?<l3>)                     # Mark this as a 3-parameter limit and parse it as two commas
                                                            # with three optional integers. The integers cannot have unnecessary
                                                            # leading zeros. The first two parameters cannot be zero at all.
                                (?<lm>-?[1-9]\d*)?,(?<lk>-?[1-9]\d*)?,(?<ln>0|-?[1-9]\d*)?
                            |
                                (?<l2>)                     # Mark this as a 2-parameter limit and parse it as one comma
                                                            # with two optional integers. The integers cannot have unnecessary
                                                            # leading zeros. The first parameter cannot be zero at all.
                                (?<lm>-?[1-9]\d*)?,(?<ln>0|-?[1-9]\d*)?
                            |
                                (?<l1>)                     # Mark this as a 1-parameter limit and parse it as a single
                                                            # integer. The integer cannot have unnecessary leading zeros.
                                (?<ln>0|-?[1-9]\d*)
                            )
                        )
                    |
                        (?<listFormat>                      # [, | and ] followed by a character or string determines the
                                                            # start, delimiter or end of list-like output, respectively.
                            [|[\]]
                            (
                                ""                          # Strings are surrounded by double quotes and, these can be
                                                            # escaped inside the string by doubling them.
                                (?<string>
                                    (?:[^""]|"""")*
                                )
                                ""
                            |
                                '?(?<char>.)                 # Characters are preceded by single quotes.
                            )
                        )
                    |
                        (?<openGroup>[({])                  # ( or { starts a new group stage.
                    |
                        (?<closeGroup>[)}])                 # ) or } ends a group stage.
                    |
                        (?<compoundStage>                   # These options introduce a compound stage, which is wrapped around
                                                            # the current one to modify its behavior.
                            [+%*_&~<>;\\]
                        )
                    |
                        (?<multiPattern>                    # A # followed by an integer indicates that this stage contains
                                                            # multiple patterns (and multiple substitutions if applicable).
                                                            # The sign indicates how those patterns are used.
                            [#]
                            (?<patternCount>
                                -?[1-9]\d*
                            )
                        )
                    |
                        (?<end>`)                           # An unescaped backtick ends the configuration string.
                        (?<remainder>.*)                    # The remainder is used as the regex (or input or substitution 
                                                            # for some configurations).
                    |
                        ""                                  # Strings are surrounded by double quotes and, these can be
                                                            # escaped inside the string by doubling them.
                        (?<stringParam>
                            ([^""]|"""")*
                        )
                        ""
                    |
                        '(?<charParam>.)                    # Characters are preceded by single quotes.
                    |
                        (?<charParam>\n)                    # Unless it's just a linefeed.
                    |
                        /                                   # Regices are surrounded by slashes and, these can be
                                                            # escaped inside the regex with the usual backslash.
                        (?<regexParam>
                            ([^\\/]|\\.)*
                        )
                        /
                        (?<regexModifier>[a-z])*
                    |
                        (?<flag>
                            !.                              # ! marks a 2-character flag.
                        |
                            .                               # All other characters are read individually and represent 
                                                            # various options.
                        )
                    )", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                    MatchCollection tokens = configTokenizer.Matches(pattern);
                    
                    bool terminated = false;

                    bool hasOpenParenthesis = false;
                    bool hasCloseParenthesis = false;

                    foreach (Match t in tokens)
                    {
                        if (t.Groups["end"].Success)
                        {
                            pattern = t.Groups["remainder"].Value;
                            terminated = true;
                            break;
                        }
                        else if (t.Groups["limit"].Success)
                        {
                            Limit limit;
                            if (t.Groups["l1"].Success)
                                limit = new Limit(int.Parse(t.Groups["ln"].Value));
                            else if (t.Groups["l2"].Success)
                            {
                                int m = t.Groups["lm"].Success ? int.Parse(t.Groups["lm"].Value) : 0;
                                int n = t.Groups["ln"].Success ? int.Parse(t.Groups["ln"].Value) : -1;
                                limit = new Limit(m, n);
                            }
                            else if (t.Groups["l3"].Success)
                            {
                                int m = t.Groups["lm"].Success ? int.Parse(t.Groups["lm"].Value) : 0;
                                int k = t.Groups["lk"].Success ? int.Parse(t.Groups["lk"].Value) : 0;
                                int n = t.Groups["ln"].Success ? int.Parse(t.Groups["ln"].Value) : -1;
                                limit = new Limit(m, k, n);
                            }
                            else
                            {
                                throw new Exception("Limit is neither l1, l2 nor l3. This shouldn't happen.");
                            }

                            limit.Negated = t.Groups["lneg"].Success;

                            config.Limits.Add(limit);
                        }
                        else if (t.Groups["stringParam"].Success)
                        {
                            config.StringParam = t.Groups["stringParam"].Value.Replace("\"\"", "\""); ;
                        }
                        else if (t.Groups["charParam"].Success)
                        {
                            config.StringParam = t.Groups["charParam"].Value;
                        }
                        else if (t.Groups["regexParam"].Success)
                        {
                            var regexOptions = new RegexOptions();
                            foreach (var c in t.Groups["regexModifier"].Captures.Cast<Capture>())
                            {
                                switch (c.Value[0])
                                {
                                case 'c': regexOptions |= RegexOptions.CultureInvariant; break;
                                case 'e': regexOptions |= RegexOptions.ECMAScript; break;
                                case 'i': regexOptions |= RegexOptions.IgnoreCase; break;
                                case 'm': regexOptions |= RegexOptions.Multiline; break;
                                case 'n': regexOptions |= RegexOptions.ExplicitCapture; break;
                                case 'r': regexOptions |= RegexOptions.RightToLeft; break;
                                case 's': regexOptions |= RegexOptions.Singleline; break;
                                case 'x': regexOptions |= RegexOptions.IgnorePatternWhitespace; break;
                                }
                            }
                            config.RegexParam = new Regex(t.Groups["regexParam"].Value, regexOptions);
                        }
                        else if (t.Groups["listFormat"].Success)
                        {
                            string value = "";
                            if (t.Groups["char"].Success)
                                value = t.Groups["char"].Value;
                            else if (t.Groups["string"].Success)
                                value = t.Groups["string"].Value.Replace("\"\"", "\"");

                            switch (t.Groups["listFormat"].Value[0])
                            {
                            case '[':
                                config.ListStart = value;
                                break;
                            case '|':
                                config.ListSeparator = value;
                                break;
                            case ']':
                                config.ListEnd = value;
                                break;
                            default:
                                throw new Exception("List format is none of [|]. This shouldn't happen.");
                            }
                        }
                        else if (t.Groups["openGroup"].Success)
                        {
                            if (hasCloseParenthesis)
                                throw new Exception("Stage configuration cannot contain both '(' and ')'.");
                            hasOpenParenthesis = true;

                            if (t.Groups["openGroup"].Value == "{")
                            {
                                compoundStack.Push(new Tuple<char, Config>('+', config));
                                config = new Config();
                            }

                            groupConfig.Push(config);
                            config = new Config();
                            groupCompoundStack.Push(compoundStack);
                            compoundStack = new Stack<Tuple<char, Config>>();
                            mode = i < sources.Count ? Modes.Replace : Modes.Count;
                            useSubstitution = false;
                            patternCount = 1;
                            stageStack.Push(new List<Stage>());
                        }
                        else if (t.Groups["closeGroup"].Success)
                        {
                            if (hasOpenParenthesis)
                                throw new Exception("Stage configuration cannot contain both '(' and ')'.");
                            hasCloseParenthesis = true;

                            if (t.Groups["closeGroup"].Value == "}")
                            {
                                compoundStack.Push(new Tuple<char, Config>('+', config));
                                config = new Config();
                            }

                            compoundStack.Push(new Tuple<char, Config>(')', config));
                            config = new Config();
                            mode = i < sources.Count ? Modes.Replace : Modes.Count;
                            useSubstitution = false;
                            patternCount = 1;
                        }
                        else if (t.Groups["compoundStage"].Success)
                        {
                            compoundStack.Push(new Tuple<char, Config>(t.Groups["compoundStage"].Value[0], config));
                            config = new Config();
                            mode = i < sources.Count ? Modes.Replace : Modes.Count;
                            useSubstitution = false;
                            patternCount = 1;
                        }
                        else if (t.Groups["multiPattern"].Success)
                        {
                            patternCount = int.Parse(t.Groups["patternCount"].Value);
                        }
                        else if (t.Groups["flag"].Success)
                        {
                            char flag = t.Groups["flag"].Value[0];
                            if (flag == '!')
                            {
                                // Handle 2-character flags.
                                flag = t.Groups["flag"].Value[1];
                                switch (flag)
                                {
                                case '_':
                                    config.OmitEmpty = true;
                                    break;
                                case '-':
                                    config.OmitGroups = true;
                                    break;
                                case '|':
                                    config.TransliterateOnce = true;
                                    break;
                                default:
                                    break;
                                }
                            }
                            else
                            {
                                // Handle 1-character flags.
                                switch (flag)
                                {
                                // Parse RegexOptions
                                case 'c':
                                    config.RegexOptions |= RegexOptions.CultureInvariant;
                                    break;
                                case 'e':
                                    config.RegexOptions |= RegexOptions.ECMAScript;
                                    break;
                                case 'i':
                                    config.RegexOptions |= RegexOptions.IgnoreCase;
                                    break;
                                case 'm':
                                    config.RegexOptions |= RegexOptions.Multiline;
                                    break;
                                case 'n':
                                    config.RegexOptions |= RegexOptions.ExplicitCapture;
                                    break;
                                case 'r':
                                    config.RegexOptions |= RegexOptions.RightToLeft;
                                    break;
                                case 's':
                                    config.RegexOptions |= RegexOptions.Singleline;
                                    break;
                                case 'x':
                                    config.RegexOptions |= RegexOptions.IgnorePatternWhitespace;
                                    break;

                                // Parse custom regex modifiers
                                case 'a':
                                    config.Anchoring = Anchoring.String;
                                    break;
                                case 'l':
                                    config.Anchoring = Anchoring.Line;
                                    break;
                                case 'p':
                                    config.UniqueMatches = UniqueMatches.KeepLast;
                                    break;
                                case 'q':
                                    config.UniqueMatches = UniqueMatches.KeepFirst;
                                    break;
                                case 'v':
                                    config.Overlaps = Overlaps.OverlappingSimple;
                                    break;
                                case 'w':
                                    config.Overlaps = Overlaps.OverlappingAll;
                                    break;
                                case 'y':
                                    config.CyclicMatches = true;
                                    break;

                                // Parse Mode
                                case 'C':
                                    mode = Modes.Count;
                                    break;
                                case 'L':
                                    mode = Modes.List;
                                    break;
                                case 'R':
                                    mode = Modes.Replace;
                                    break;
                                case 'G':
                                    mode = Modes.Grep;
                                    break;
                                case 'A':
                                    mode = Modes.AntiGrep;
                                    break;
                                case 'S':
                                    mode = Modes.Split;
                                    break;
                                case 'T':
                                    config.CyclicTransliteration = false;
                                    mode = Modes.Transliterate;
                                    break;
                                case 'Y':
                                    config.CyclicTransliteration = true;
                                    mode = Modes.Transliterate;
                                    break;
                                case 'N':
                                    config.SortNumerically = true;
                                    mode = Modes.Sort;
                                    break;
                                case 'O':
                                    config.SortNumerically = false;
                                    mode = Modes.Sort;
                                    break;
                                case 'D':
                                    mode = Modes.Deduplicate;
                                    break;
                                case 'I':
                                    mode = Modes.Position;
                                    break;
                                case 'V':
                                    mode = Modes.Reverse;
                                    break;
                                case 'P':
                                    mode = Modes.Pad;
                                    break;

                                // Global configuration
                                case '.':
                                    silent = true;
                                    break;

                                // Flags affecting how subsequent lines are read
                                case '$':
                                    useSubstitution = true;
                                    break;

                                // General configuration for atomic stages
                                case '?':
                                    config.Random = true;
                                    break;
                                case '@':
                                    config.SingleRandomMatch = true;
                                    break;
                                case '=':
                                    config.InvertMatches = true;
                                    break;
                                case '^':
                                    config.Reverse = true;
                                    break;
                                case ':':
                                    config.InputAsRegex = true;
                                    break;
                                default:
                                    break;
                                }
                            }
                        }
                    }

                    if (!terminated)
                        throw new Exception("Source contains only escaped backticks.");
                }

                if (mode == Modes.Replace)
                    useSubstitution = true;
                
                Stage stage = null;
                var patterns = new List<string>();
                var substitutions = new List<string>();

                patterns.Add(pattern);

                string substitution = "$&";
                if (!config.InvertMatches && useSubstitution)
                    substitution = i < sources.Count ? sources[i++] : "";
                substitutions.Add(substitution);

                if (Math.Abs(patternCount) > 1)
                {
                    if (config.InputAsRegex)
                        throw new Exception("Can't use : in conjunction with #.");

                    config.Greedy = patternCount < 0;
                    for (int k = 1; k < Math.Abs(patternCount); ++k)
                    {
                        if (i == sources.Count)
                            throw new Exception("Not enough lines to reach pattern count");

                        patterns.Add(sources[i++]);

                        substitution = "$&";
                        if (!config.InvertMatches && useSubstitution)
                            substitution = i < sources.Count ? sources[i++] : "";
                        substitutions.Add(substitution);
                    }
                }

                string separatorSubstitutionSource = "$&";

                if (config.InvertMatches && useSubstitution)
                    separatorSubstitutionSource = i < sources.Count ? sources[i++] : "";

                if (!config.InvertMatches
                 && patternCount == 1 
                 && patterns[0] == "")
                {
                    switch (mode)
                    {
                    case Modes.Deduplicate:
                    case Modes.Sort:
                    case Modes.Reverse:
                        patterns[0] = "(?m:^.*$)";
                        break;
                    case Modes.Transliterate:
                        patterns[0] = @"\A(?s:.*)\z";
                        break;
                    }
                }

                switch (mode)
                {
                case Modes.Count:
                    stage = new CountStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.List:
                    stage = new ListStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Replace:
                    stage = new ReplaceStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Split:
                    stage = new SplitStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Grep:
                    stage = new GrepStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.AntiGrep:
                    stage = new AntiGrepStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Transliterate:
                    stage = new TransliterateStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Sort:
                    stage = new SortStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Deduplicate:
                    stage = new DeduplicateStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Position:
                    stage = new PositionStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Reverse:
                    stage = new ReverseStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                case Modes.Pad:
                    stage = new PadStage(config, History, patterns, substitutions, separatorSubstitutionSource);
                    break;
                default:
                    throw new NotImplementedException();
                }
                                
                while (compoundStack.Count > 0 
                    || i >= sources.Count && stageStack.Count > 1)
                {
                    if (compoundStack.Count == 0)
                        compoundStack.Push(new Tuple<char, Config>(')', new Config()));

                    var compoundStage = compoundStack.Pop();

                    char compoundType = compoundStage.Item1;
                    Config compoundConfig = compoundStage.Item2;

                    switch (compoundType)
                    {
                    case ')':
                        if (groupConfig.Count > 0)
                            compoundConfig.Merge(groupConfig.Pop());

                        List<Stage> stages = stageStack.Pop();
                        stages.Add(stage);

                        InheritConfig(stages, compoundConfig);
                        stage = new GroupStage(compoundConfig, History, stages);

                        if (stageStack.Count == 0)
                            stageStack.Push(new List<Stage>());
                        
                        if (groupCompoundStack.Count > 0)
                            foreach (var compound in groupCompoundStack.Pop().Reverse())
                                compoundStack.Push(compound);
                        
                        break;
                    case '>':
                        InheritConfig(stage, compoundConfig);
                        stage = new OutputStage(compoundConfig, stage);
                        break;
                    case '<':
                        compoundConfig.PrePrint = true;
                        InheritConfig(stage, compoundConfig);
                        stage = new OutputStage(compoundConfig, stage);
                        break;
                    case ';':
                        compoundConfig.PrintOnlyIfChanged = true;
                        InheritConfig(stage, compoundConfig);
                        stage = new OutputStage(compoundConfig, stage);
                        break;
                    case '\\':
                        if (compoundConfig.StringParam == null)
                            compoundConfig.StringParam = "\n";
                        InheritConfig(stage, compoundConfig);
                        stage = new OutputStage(compoundConfig, stage);
                        break;
                    case '*':
                        InheritConfig(stage, compoundConfig);
                        stage = new DryRunStage(compoundConfig, stage);
                        break;
                    case '+':
                        InheritConfig(stage, compoundConfig);
                        stage = new LoopStage(compoundConfig, History, stage);
                        break;
                    case '%':
                        InheritConfig(stage, compoundConfig);
                        stage = new PerLineStage(compoundConfig, History, stage);
                        break;
                    case '_':
                        InheritConfig(stage, compoundConfig);
                        stage = new MatchMaskStage(compoundConfig, History, stage);
                        break;
                    case '&':
                        InheritConfig(stage, compoundConfig);
                        stage = new ConditionalStage(compoundConfig, stage);
                        break;
                    case '~':
                        InheritConfig(stage, compoundConfig);
                        stage = new EvalStage(compoundConfig, History, stage);
                        break;
                    }
                }
                stageStack.Peek().Add(stage);
            }

            {
                // At this point, the stageStack should only hold a single list.
                // If the final stage isn't already an output stage, and the
                // silent flag wasn't set, wrap it in an output stage.
                List<Stage> stages = stageStack.Pop();
                if (!(stages.Last() is OutputStage && !stages.Last().Config.PrePrint) && !silent)
                {
                    Stage stage = stages.Last();
                    stage = new OutputStage(new Config(), stage);
                    stages.RemoveAt(stages.Count - 1);
                    stages.Add(stage);
                }
                StageTree = new GroupStage(new Config(), History, stages);
            }
        }

        private static void InheritConfig(IList<Stage> stages, Config config)
        {
            foreach (var stage in stages)
                InheritConfig(stage, config);
        }

        private static void InheritConfig(Stage stage, Config config)
        {
            stage.Config.Inherit(config);
            if (stage is GroupStage)
                InheritConfig(((GroupStage)stage).Stages, config);
            else if (stage is OutputStage)
                InheritConfig(((OutputStage)stage).ChildStage, config);
            else if (stage is DryRunStage)
                InheritConfig(((DryRunStage)stage).ChildStage, config);
            else if (stage is LoopStage)
                InheritConfig(((LoopStage)stage).ChildStage, config);
            else if (stage is PerLineStage)
                InheritConfig(((PerLineStage)stage).ChildStage, config);
        }


        public void Execute(string input, TextWriter output)
        {
            History.RegisterResult(0, input);
            StageTree.Execute(input, output);
        }
    }
}