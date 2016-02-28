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
        public bool Loop { get; set; }
        public bool IterationSilent { get; set; }
        public bool IterationTrailingLinefeed { get; set; }

        public List<int> Limits { get; set; }

        // Options for Match mode
        public bool Overlapping { get; set; }
        public bool PrintMatches { get; set; }

        // Options for Split mode
        public bool OmitEmpty { get; set; }

        // Options for Replace mode

        public Options(string optionString, Modes defaultMode)
        {
            TrailingLinefeed = true;
            IterationSilent = true;
            IterationTrailingLinefeed = true;

            Mode = defaultMode;

            var tokenizer = new Regex(@"\G(?:    # Use \G to ensure that the tokens cover the entire string.
                        (?<limit>\d+)            # All integers are read as limits.
                    |
                        .                        # All other characters are read individually and represent various options.
                    )", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            MatchCollection tokens = tokenizer.Matches(optionString);

            foreach (Match t in tokens)
            {
                if (t.Groups["limit"].Success)
                {
                    Limits.Add(int.Parse(t.Groups["limit"].Value));
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
                    case '+':
                        Loop = true;
                        break;
                    default:
                        break;
                    }
                }
            }
        }

        public void Inherit(Options other)
        {
            RegexOptions ^= other.RegexOptions;
            // TODO: Inherit limits as well?
        }

    }
}
