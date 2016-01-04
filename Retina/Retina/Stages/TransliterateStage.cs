using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    public class TransliterateStage : RegexStage
    {
        public string Replacement { get; set; }

        private string From { get; set; }
        private string To { get; set; }

        public TransliterateStage(Options options, string pattern) : base(options)
        {
            var source = new StringBuilder(pattern);

            var from = new StringBuilder();
            var to = new StringBuilder();

            ParseCharacterSet(from, source);
            ParseCharacterSet(to, source);

            From = from.ToString();
            To = to.ToString();

            Pattern = new Regex(source.Length == 0 ? @"[\s\S]+" : source.ToString(), options.RegexOptions);
        }

        private void ParseCharacterSet(StringBuilder setBuilder, StringBuilder source)
        {
            int i = 0;
            bool rangePossible = false;
            while (source.Length > 0)
            {
                char c = source[0];
                source.Remove(0, 1);
                switch (c)
                {
                case '`':
                    return;
                case '\\':
                    if (source.Length == 0)
                    {
                        setBuilder.Append(c);
                    }
                    else
                    {
                        c = source[0];
                        source.Remove(0, 1);
                        rangePossible = true;
                        switch (c)
                        {
                        // Character escapes
                        case 'a': setBuilder.Append('\a'); break;
                        case 'b': setBuilder.Append('\b'); break;
                        case 'f': setBuilder.Append('\f'); break;
                        case 'n': setBuilder.Append('\n'); break;
                        case 'r': setBuilder.Append('\r'); break;
                        case 't': setBuilder.Append('\t'); break;
                        case 'v': setBuilder.Append('\v'); break;

                        // Any other character is treated as a literal.
                        default: setBuilder.Append(c); break;
                        }
                    }
                    break;
                // Character classes
                case 'd': 
                    setBuilder.Append("0123456789");
                    rangePossible = false;
                    break;
                case 'H':
                    setBuilder.Append("0123456789ABCDEF");
                    rangePossible = false;
                    break;
                case 'h':
                    setBuilder.Append("0123456789abcdef");
                    rangePossible = false;
                    break;
                case 'L':
                    setBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                    rangePossible = false;
                    break;
                case 'l':
                    setBuilder.Append("abcdefghijklmnopqrstuvwxyz");
                    rangePossible = false;
                    break;
                case 'w':
                    setBuilder.Append("_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                    rangePossible = false;
                    break;
                case 'p':
                    // Printable ASCII
                    setBuilder.Append(@" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
                    rangePossible = false;
                    break;
                // Ranges
                case '-':
                    // TODO: Make escapes work as the end of a range.
                    if (rangePossible && source.Length > 0)
                    {
                        char end = source[0];
                        source.Remove(0, 1);
                        c = setBuilder[setBuilder.Length - 1];
                        setBuilder.Remove(setBuilder.Length - 1, 1);
                        int step = end > c ? 1 : -1;
                        for (; c != end; c = (char)((int)c + step))
                            setBuilder.Append(c);
                        setBuilder.Append(c);
                        rangePossible = false;
                    }
                    else
                    {
                        setBuilder.Append(c);
                        rangePossible = true;
                    }
                    break;
                default:
                    setBuilder.Append(c);
                    rangePossible = true;
                    break;
                }
            }
        }

        protected override StringBuilder Process(string input)
        {
            var builder = new StringBuilder();

            int n = To.Length;
            int i = 0;

            var matches = Pattern.Matches(input).Cast<Match>();
            if (Pattern.Options.HasFlag(RegexOptions.RightToLeft))
                matches = matches.Reverse();

            foreach (Match m in matches)
            {
                builder.Append(input.Substring(i, m.Index-i));
                foreach (char c in m.Value)
                {
                    int j = From.IndexOf(c);
                    if (j < 0)
                        builder.Append(c);
                    else if (n > 0)
                        builder.Append(To[Math.Min(n - 1, j)]);
                }
                i = m.Index + m.Length;
            }

            builder.Append(input.Substring(i));

            return builder;
        }
    }
}
