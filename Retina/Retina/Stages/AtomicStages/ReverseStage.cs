using System.Collections.Generic;
using System.Linq;
using System.IO;
using Retina.Configuration;
using Retina.Extensions;

namespace Retina.Stages
{
    public class ReverseStage : AtomicStage
    {
        public ReverseStage(Config config, List<string> patterns, List<string> substitutions, string separatorSubstitutionSource)
            : base(config, patterns, substitutions, separatorSubstitutionSource) { }

        protected override string Process(string input, TextWriter output)
        {
            var separators = Separators.Select(s => s.Match.Value);

            var charLimit = Config.GetLimit(1);
            var reversals = Matches.Select(m =>
            {
                var charStack = new Stack<char>();
                m.Replacement.Each((c, i) =>
                {
                    if (charLimit.IsInRange(i, m.Replacement.Length))
                        charStack.Push(c);
                });

                if (Config.Random)
                {
                    var charList = charStack.ToList();
                    charList.Shuffle();
                    charStack = new Stack<char>(charList);
                }

                return new string(m.Replacement.Select(
                    (c, i) => charLimit.IsInRange(i, m.Replacement.Length) ? charStack.Pop() : c
                ).ToArray());
            });

            if (Config.Reverse)
                reversals = reversals.Reverse();

            return separators.Riffle(reversals);
        }
    }
}
