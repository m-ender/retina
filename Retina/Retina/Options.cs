using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina
{
    public class Options
    {
        public RegexOptions RegexOptions { get; set; }

        public Modes Mode { get; set; }

        // General options
        public bool? Silent { get; set; }
        public bool TrailingLinefeed { get; set; }
        public bool PerLine { get; set; }
        public bool Loop { get; set; }
        public bool IterationSilent { get; set; }
        public bool IterationTrailingLinefeed { get; set; }
        public bool IterationPerLine { get; set; }

        public List<int> Limits { get; set; }
        public List<LimitFlags> LFlags { get; set; }

        // Options for Match mode
        public bool Overlapping { get; set; }
        public bool PrintMatches { get; set; }

        // Options for Split mode
        public bool OmitEmpty { get; set; }
        public bool OmitGroups { get; set; }

        // Options for Replace mode

        // Options for Sort mode
        public bool SortNumerically { get; set; }
        public bool SortReverse { get; set; }
        public bool UseSubstitution { get; set; }

        public Options(string optionString, Modes defaultMode)
        {
            TrailingLinefeed = true;
            IterationSilent = true;
            IterationTrailingLinefeed = true;

            Limits = new List<int>();
            LFlags = new List<LimitFlags>();

            Mode = defaultMode;

            LimitFlags? currentFlags = null;

            var tokenizer = new Regex(@"\G(?:    # Use \G to ensure that the tokens cover the entire string.
                        (?<limit>0|-?\d+)        # All integers are read as limits, but leading zeroes are read individually.
                    |
                        .                        # All other characters are read individually and represent various options.
                    )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            MatchCollection tokens = tokenizer.Matches(optionString);

            foreach (Match t in tokens)
            {
                if (t.Groups["limit"].Success)
                {
                    Limits.Add(int.Parse(t.Groups["limit"].Value));
                    if (currentFlags != null)
                        LFlags.Add((LimitFlags)currentFlags);
                    currentFlags = LimitFlags.Less | LimitFlags.Equals;
                }
                else
                {
                    switch (t.Value[0])
                    {
                    // Parse RegexOptions
                    case 'c':
                        RegexOptions ^= RegexOptions.CultureInvariant;
                        break;
                    case 'e':
                        RegexOptions ^= RegexOptions.ECMAScript;
                        break;
                    case 'i':
                        RegexOptions ^= RegexOptions.IgnoreCase;
                        break;
                    case 'm':
                        RegexOptions ^= RegexOptions.Multiline;
                        break;
                    case 'n':
                        RegexOptions ^= RegexOptions.ExplicitCapture;
                        break;
                    case 'r':
                        RegexOptions ^= RegexOptions.RightToLeft;
                        break;
                    case 's':
                        RegexOptions ^= RegexOptions.Singleline;
                        break;
                    case 'x':
                        RegexOptions ^= RegexOptions.IgnorePatternWhitespace;
                        break;

                    // Parse Mode
                    case 'M':
                        Mode = Modes.Match;
                        break;
                    case 'R':
                        Mode = Modes.Replace;
                        break;
                    case 'G':
                        Mode = Modes.Grep;
                        break;
                    case 'A':
                        Mode = Modes.AntiGrep;
                        break;
                    case 'S':
                        Mode = Modes.Split;
                        break;
                    case 'T':
                        Mode = Modes.Transliterate;
                        break;
                    case 'O':
                        Mode = Modes.Sort;
                        break;

                    // General options
                    case ';':
                        if (Loop)
                            IterationSilent = true;
                        else
                            Silent = true;
                        break;
                    case ':':
                        if (Loop)
                            IterationSilent = false;
                        else
                            Silent = false;
                        break;
                    case '%':
                        if (Loop)
                            IterationPerLine = true;
                        else
                            PerLine = true;
                        break;
                    case '\\': // Implies :
                        if (Loop)   
                        {
                            IterationSilent = false;
                            IterationTrailingLinefeed = false;
                        }
                        else
                        {
                            Silent = false;
                            TrailingLinefeed = false;
                        }
                        break;
                    case '<':
                        if (currentFlags == null)
                            throw new Exception("Cannot use < before setting a limit.");
                        currentFlags = LimitFlags.Less | LimitFlags.Equals;
                        break;
                    case '=':
                        if (currentFlags == null)
                            throw new Exception("Cannot use = before setting a limit.");
                        currentFlags = LimitFlags.Equals;
                        break;
                    case '>':
                        if (currentFlags == null)
                            throw new Exception("Cannot use > before setting a limit.");
                        currentFlags = LimitFlags.Greater;
                        break;
                    case '~':
                        if (currentFlags == null)
                            throw new Exception("Cannot use ~ before setting a limit.");
                        currentFlags = LimitFlags.Less | LimitFlags.Greater;
                        break;

                    // Mode-specific options
                    case '!':
                        PrintMatches = true;
                        break;
                    case '&':
                        Overlapping = true;
                        break;
                    case '_':
                        OmitEmpty = true;
                        break;
                    case '-':
                        OmitGroups = true;
                        break;
                    case '+':
                        Loop = true;
                        break;
                    case '#':
                        SortNumerically = true;
                        break;
                    case '^':
                        SortReverse = true;
                        break;
                    case '$':
                        UseSubstitution = true;
                        break;
                    default:
                        break;
                    }
                }
            }

            if (currentFlags != null)
                LFlags.Add((LimitFlags)currentFlags);
        }

        public void Inherit(Options other)
        {
            RegexOptions ^= other.RegexOptions;
            // TODO: Inherit limits as well?
        }

        public bool IsInRange(int limitIndex, int value, int count)
        {
            if (limitIndex >= Limits.Count)
                return true;

            var limit = Limits[limitIndex];
            var flags = LFlags[limitIndex];

            if (limit == 0)
                return true;

            if (limit < 0)
                limit = count + limit + 1;
            
            return flags.HasFlag(LimitFlags.Less)    && value <  limit-1
                    || flags.HasFlag(LimitFlags.Equals)  && value == limit-1
                    || flags.HasFlag(LimitFlags.Greater) && value >  limit-1;
        }
    }
}
