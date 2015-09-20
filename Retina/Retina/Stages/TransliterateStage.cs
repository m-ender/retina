using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class TransliterateStage : RegexStage
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

            Pattern = new Regex(source.Length == 0 ? @"[\s\S]" : source.ToString());
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

                        // Character class escapes
                        case 'd':
                            setBuilder.Append("0123456789");
                            rangePossible = false;
                            break;
                        case 'w':
                            setBuilder.Append("_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
                            rangePossible = false;
                            break;

                        // Any other character is treated as a literal.
                        default: setBuilder.Append(c); break;
                        }
                    }
                    break;
                case '-':
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

            foreach (char c in input)
            {
                int i = From.IndexOf(c);
                if (i < 0)
                    builder.Append(c);
                else if (n > 0)
                    builder.Append(To[Math.Min(n - 1, i)]);
            }

            return builder;
        }
    }
}
