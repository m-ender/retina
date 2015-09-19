using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina
{
    class Options
    {
        public RegexOptions RegexOptions { get; set; }

        public Modes Mode { get; set; }

        // General options
        public bool Silent { get; set; }

        // Options for Match mode
        public bool Overlapping { get; set; }
        public bool PrintMatches { get; set; }

        // Options for Split mode
        public bool OmitEmpty { get; set; }

        // Options for Replace mode

        // Options for control flow
        public int OpenLoops { get; set; }
        public int CloseLoops { get; set; }

        public Options(string optionString, Modes defaultMode, bool last = false)
        {
            Mode = defaultMode;
            Silent = !last;

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

                // General options
                case ';':
                    Silent = true;
                    break;
                case ':':
                    Silent = false;
                    break;
                case '(':
                    ++OpenLoops;
                    break;
                case ')':
                    ++CloseLoops;
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
                    ++OpenLoops;
                    ++CloseLoops;
                    break;
                default:
                    break;
                }
            }
        }

    }
}
