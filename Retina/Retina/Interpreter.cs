using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Retina.Stages;

namespace Retina
{
    public class Interpreter
    {
        private Stage StageTree;

        public Interpreter (List<string> sources)
	    {
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
            var groupConfig = new Stack<Configuration>();
            
            // Within each stage, we'll also need to keep track of a stack of compound
            // stages that will eventually be wrapped around that stage. This is because
            // the stages wrap from right to left, so we need to apply them in the
            // reverse order they are parsed. However, compounds outside of a '(' will
            // only need to be applied once we get to the ')', so like the '(' config,
            // we need to remember the compounds we've already seen. That's what this
            // messy stack of stacks is for.
            var groupCompoundStack = new Stack<Stack<Tuple<char, Configuration>>>();

            // A global option that affects whether the whole program will be wrapped
            // in an output stage or not.
            var silent = false;

            int i = 0;
            while (i < sources.Count)
            {
                string pattern = sources[i++];

                Modes mode = i < sources.Count ? Modes.Replace : Modes.Match;

                Configuration config = new Configuration();

                // Compound stages will be wrapped around this stage once we're done
                // constructing it. However, they will be process from right to left
                // so we keep track of them on a stack. Each stack element holds the
                // type of compound stage as the character that represents it, together
                // with the configuration that was immediately left of it.
                // Groups will enter this stack at the point of their closing ')'.
                var compoundStack = new Stack<Tuple<char, Configuration>>();

                // Backticks indicate a leading configuration string.
                if (pattern.Contains("`"))
                {
                    var configTokenizer = new Regex(@"\G(?: # Use \G to ensure that the tokens cover the entire string.
                        (?<limit>
                            (?<lneg>~)?                     # Any limit can be negated with an optional ~.
                            (?:
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
                        (?<openGroup>[({])                  # ( or { starts a new group stage.
                    |
                        (?<closeGroup>[)}])                 # ) or } ends a group stage.
                    |
                        (?<compoundStage>                   # These options introduce a compound stage, which is wrapped around
                                                            # the current one to modify its behavior.
                            [+%*]
                        |
                            (?=[:;\\])                      # Output stages can be introduced with either : or ; (print only if
                                                            # changed), and take an optional parameter \, indicating that a
                                                            # trailing linefeed should be printed. \ on its own is also valid as
                                                            # a shorthand for :\.
                            (?<compoundOutput>
                                (?<printOnlyIfChanged>)
                                ;
                                (?<trailingLF>\\)?
                            |
                                :?
                                (?<trailingLF>\\)?
                            )
                        )
                                                            
                    |
                        (?<end>`)                           # An unescaped backtick ends the configuration string.
                        (?<remainder>.*)                    # The remainder is used as the regex (or input or substitution 
                                                            # for some configurations).
                    |
                        .                                   # All other characters are read individually and represent 
                                                            # various options.
                    )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

                    MatchCollection tokens = configTokenizer.Matches(pattern);
                    
                    bool terminated = false;

                    bool hasOpenParenthesis = false;
                    bool hasCloseParenthesis = false;

                    foreach (Match t in tokens)
                    {
                        if (t.Groups["limit"].Success)
                        {
                            Limit limit;
                            if (t.Groups["l1"].Success)
                                limit = new Limit(int.Parse(t.Groups["ln"].Value));
                            else if(t.Groups["l2"].Success)
                            {
                                int m = t.Groups["lm"].Success ? int.Parse(t.Groups["lm"].Value) : 0;
                                int n = t.Groups["ln"].Success ? int.Parse(t.Groups["ln"].Value) : -1;
                                limit = new Limit(m, n);
                            }
                            else if(t.Groups["l3"].Success)
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
                        else if (t.Groups["end"].Success)
                        {
                            pattern = t.Groups["remainder"].Value;
                            terminated = true;
                            break;
                        }
                        else if (t.Groups["openGroup"].Success)
                        {
                            if (hasCloseParenthesis)
                                throw new Exception("Stage configuration cannot contain both '(' and ')'.");
                            hasOpenParenthesis = true;

                            if (t.Groups["openGroup"].Value == "{")
                            {
                                compoundStack.Push(new Tuple<char, Configuration>('+', config));
                                config = new Configuration();
                            }

                            groupConfig.Push(config);
                            config = new Configuration();
                            groupCompoundStack.Push(compoundStack);
                            compoundStack = new Stack<Tuple<char, Configuration>>();
                            mode = i < sources.Count ? Modes.Replace : Modes.Match;
                            stageStack.Push(new List<Stage>());
                        }
                        else if (t.Groups["closeGroup"].Success)
                        {
                            if (hasOpenParenthesis)
                                throw new Exception("Stage configuration cannot contain both '(' and ')'.");
                            hasCloseParenthesis = true;

                            if (t.Groups["openGroup"].Value == "}")
                            {
                                compoundStack.Push(new Tuple<char, Configuration>('+', config));
                                config = new Configuration();
                            }

                            compoundStack.Push(new Tuple<char, Configuration>(')', config));
                            config = new Configuration();
                            mode = i < sources.Count ? Modes.Replace : Modes.Match;
                        }
                        else if (t.Groups["compoundStage"].Success)
                        {
                            if (t.Groups["compoundOutput"].Success)
                            {
                                config.PrintOnlyIfChanged = t.Groups["printOnlyIfChanged"].Success;
                                config.TrailingLinefeed = t.Groups["trailingLF"].Success;
                                compoundStack.Push(new Tuple<char, Configuration>(':', config));
                            }
                            else
                            {
                                compoundStack.Push(new Tuple<char, Configuration>(t.Groups["compoundStage"].Value[0], config));
                                config = new Configuration();
                            }
                        }
                        else
                        {
                            switch (t.Value[0])
                            {
                            // Parse RegexOptions
                            case 'c':
                                config.RegexOptions ^= RegexOptions.CultureInvariant;
                                break;
                            case 'e':
                                config.RegexOptions ^= RegexOptions.ECMAScript;
                                break;
                            case 'i':
                                config.RegexOptions ^= RegexOptions.IgnoreCase;
                                break;
                            case 'm':
                                config.RegexOptions ^= RegexOptions.Multiline;
                                break;
                            case 'n':
                                config.RegexOptions ^= RegexOptions.ExplicitCapture;
                                break;
                            case 'r':
                                config.RegexOptions ^= RegexOptions.RightToLeft;
                                break;
                            case 's':
                                config.RegexOptions ^= RegexOptions.Singleline;
                                break;
                            case 'x':
                                config.RegexOptions ^= RegexOptions.IgnorePatternWhitespace;
                                break;

                            // Parse Mode
                            case 'M':
                                mode = Modes.Match;
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
                                mode = Modes.Transliterate;
                                break;
                            case 'O':
                                mode = Modes.Sort;
                                break;
                            case 'D':
                                mode = Modes.Deduplicate;
                                break;

                            // Global configuration
                            case '.':
                                silent = true;
                                break;

                            // Configuration for compound stages
                            case '\\':
                                config.TrailingLinefeed = true;
                                break;

                            // General configuration for atomic stages
                            case '&':
                                config.Overlapping = true;
                                break;
                            case '=':
                                config.Unique = true;
                                break;
                            case '$':
                                config.UseSubstitution = true;
                                break;

                            // Mode-specific configuration
                            case '!':
                                config.PrintMatches = true;
                                break;
                            case '_':
                                config.OmitEmpty = true;
                                break;
                            // TODO: Conflicts with negative limits.
                            case '-':
                                config.OmitGroups = true;
                                break;
                            case '#':
                                config.SortNumerically = true;
                                break;
                            case '^':
                                config.SortReverse = true;
                                break;
                            default:
                                break;
                            }
                        }
                    }

                    if (!terminated)
                        throw new Exception("Source contains only escaped backticks.");
                }
                
                Stage stage = null;
                string replacement;
                switch (mode)
                {
                case Modes.Match:
                    stage = new MatchStage(config, pattern);
                    break;
                case Modes.Replace:
                    replacement = i < sources.Count ? sources[i++] : "";
                    stage = new ReplaceStage(config, pattern, replacement);
                    break;
                case Modes.Split:
                    stage = new SplitStage(config, pattern);
                    break;
                case Modes.Grep:
                    stage = new GrepStage(config, pattern);
                    break;
                case Modes.AntiGrep:
                    stage = new AntiGrepStage(config, pattern);
                    break;
                case Modes.Transliterate:
                    stage = new TransliterateStage(config, pattern);
                    break;
                case Modes.Sort:
                    if (config.UseSubstitution)
                        replacement = i < sources.Count ? sources[i++] : "";
                    else
                        replacement = "$&";
                    stage = new SortStage(config, pattern, replacement);
                    break;
                case Modes.Deduplicate:
                    if (config.UseSubstitution)
                        replacement = i < sources.Count ? sources[i++] : "";
                    else
                        replacement = "$&";
                    stage = new DeduplicateStage(config, pattern, replacement);
                    break;
                default:
                    throw new NotImplementedException();
                }
                                
                while (compoundStack.Count > 0 
                    || i >= sources.Count && stageStack.Count > 1)
                {
                    if (compoundStack.Count == 0)
                        compoundStack.Push(new Tuple<char, Configuration>(')', new Configuration()));

                    var compoundStage = compoundStack.Pop();

                    char compoundType = compoundStage.Item1;
                    Configuration compoundConfig = compoundStage.Item2;

                    switch (compoundType)
                    {
                    case ')':
                        if (groupConfig.Count > 0)
                            compoundConfig.Merge(groupConfig.Pop());

                        List<Stage> stages = stageStack.Pop();
                        stages.Add(stage);

                        InheritConfig(stages, compoundConfig);
                        stage = new GroupStage(compoundConfig, stages);

                        if (stageStack.Count == 0)
                            stageStack.Push(new List<Stage>());
                        
                        if (groupCompoundStack.Count > 0)
                            foreach (var compound in groupCompoundStack.Pop().Reverse())
                                compoundStack.Push(compound);
                        
                        break;
                    case ':':
                        InheritConfig(stage, compoundConfig);
                        stage = new OutputStage(compoundConfig, stage);
                        break;
                    case '*':
                        InheritConfig(stage, compoundConfig);
                        stage = new DryRunStage(compoundConfig, stage);
                        break;
                    case '+':
                        InheritConfig(stage, compoundConfig);
                        stage = new LoopStage(compoundConfig, stage);
                        break;
                    case '%':
                        InheritConfig(stage, compoundConfig);
                        stage = new PerLineStage(compoundConfig, stage);
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
                if (!(stages.Last() is OutputStage) && !silent)
                {
                    Stage stage = stages.Last();
                    stage = new OutputStage(new Configuration(), stage);
                    stages.RemoveAt(stages.Count - 1);
                    stages.Add(stage);
                }
                StageTree = new GroupStage(new Configuration(), stages);
            }
        }

        private static void InheritConfig(IList<Stage> stages, Configuration config)
        {
            foreach (var stage in stages)
                InheritConfig(stage, config);
        }

        private static void InheritConfig(Stage stage, Configuration config)
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
            StageTree.Execute(input, output);
        }
    }
}