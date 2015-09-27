using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Retina.Replace;

namespace Retina.Stages
{
    public class ReplaceStage : RegexStage
    {
        public List<Token> Tokens { get; set; }

        public ReplaceStage(Options options, string pattern, string replacement) : base(options, pattern)
        {
            ParseReplacement(replacement);
        }

        private void ParseReplacement(string replacement)
        {
            Tokens = new List<Token>();

            Tokens.Add(new Literal(replacement));
        }
            
        protected override StringBuilder Process(string input)
        {
            var builder = new StringBuilder();

            int i = 0;

            foreach (Match m in Pattern.Matches(input))
            {
                builder.Append(input.Substring(i, m.Index-i));
                foreach (Token t in Tokens)
                    builder.Append(t.Process(m));
                i = m.Index + m.Length;
            }

            builder.Append(input.Substring(i));

            return builder;
        }
    }
}
