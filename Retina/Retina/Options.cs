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
        [DefaultValue(RegexOptions.None)]
        public RegexOptions RegexOptions { get; set; }

        [DefaultValue(Modes.Match)]
        public Modes Mode { get; set; }

        // General options
        [DefaultValue(true)]
        public bool Silent { get; set; }

        // Options for Match mode
        [DefaultValue(false)]
        public bool Overlapping { get; set; }
        [DefaultValue(false)]
        public bool PrintMatches { get; set; }

        // Options for Split mode
        [DefaultValue(false)]
        public bool OmitEmpty { get; set; }

        // Options for Replace mode
        [DefaultValue(false)]
        public bool Loop { get; set; }
        [DefaultValue(false)]
        public bool Trace { get; set; }

        public Options(string optionString, bool replaceMode = false, bool last = false)
        {
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
                case '?':
                    Trace = true;
                    break;
                default:
                    break;
                }
            }

            if (replaceMode)
                Mode = Modes.Replace;
        }

    }
}
