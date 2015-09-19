﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina.Stages
{
    class GrepStage : RegexStage
    {
        public GrepStage(Options options, Regex pattern) : base(options, pattern) { }

        public override string Execute(string input)
        {
            string line;
            var stringReader = new StringReader(input);

            var builder = new StringBuilder();

            bool first = true;
            while ((line = stringReader.ReadLine()) != null)
            {
                if (Pattern.IsMatch(line) ^ (Options.Mode == Modes.AntiGrep))
                {
                    if (!first) builder.Append("\n");
                    first = false;
                    builder.Append(line);
                }
            }

            string result = builder.ToString();

            if (!Options.Silent)
                Console.WriteLine(result);

            return result;
        }
    }
}
