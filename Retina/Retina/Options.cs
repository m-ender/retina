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

        // Options for Match mode
        public bool Overlapping { get; set; }
        public bool PrintMatches { get; set; }

        // Options for Split mode
        public bool OmitEmpty { get; set; }

        // Options for Replace mode

        // Options for control flow
        public int OpenLoops { get; set; }
        public List<bool?> CloseLoopsSilent { get; set; }
        public List<bool> CloseLoopsTrailingLinefeed { get; set; }

        public Options(string optionString, Modes defaultMode)
        {
            Mode = defaultMode;
            CloseLoopsSilent = new List<bool?>();
            CloseLoopsTrailingLinefeed = new List<bool>();

            TrailingLinefeed = true;

            foreach (char c in optionString)
            {
                switch (c)
                {
                // Parse RegexOptions
                case 'c':
                    RegexOptions |= RegexOptions.CultureInvariant;
                    break;
                case 'e':
                    RegexOptions |= RegexOptions.ECMAScript;
                    break;
                case 'i':
                    RegexOptions |= RegexOptions.IgnoreCase;
                    break;
                case 'm':
                    RegexOptions |= RegexOptions.Multiline;
                    break;
                case 'n':
                    RegexOptions |= RegexOptions.ExplicitCapture;
                    break;
                case 'r':
                    RegexOptions |= RegexOptions.RightToLeft;
                    break;
                case 's':
                    RegexOptions |= RegexOptions.Singleline;
                    break;
                case 'x':
                    RegexOptions |= RegexOptions.IgnorePatternWhitespace;
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
                    if (CloseLoopsSilent.Count == 0)
                        Silent = true;
                    else
                        CloseLoopsSilent[CloseLoopsSilent.Count - 1] = true;
                    break;
                case ':':
                    if (CloseLoopsSilent.Count == 0)
                        Silent = false;
                    else
                        CloseLoopsSilent[CloseLoopsSilent.Count - 1] = false;
                    break;
                case '\\':
                    if (CloseLoopsTrailingLinefeed.Count == 0)
                        TrailingLinefeed = false;
                    else
                        CloseLoopsTrailingLinefeed[CloseLoopsTrailingLinefeed.Count - 1] = false;
                    break;
                case '(':
                    ++OpenLoops;
                    break;
                case ')':
                    CloseLoopsSilent.Add(null);
                    CloseLoopsTrailingLinefeed.Add(true);
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
                    // Short-hand for ()
                    ++OpenLoops;
                    CloseLoopsSilent.Add(null);
                    CloseLoopsTrailingLinefeed.Add(true);
                    break;
                default:
                    break;
                }
            }
        }

    }
}
