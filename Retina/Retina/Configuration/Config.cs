using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Retina.Configuration
{
    public class Config
    {
        public RegexOptions RegexOptions { get; set; }

        // General configuration        
        public List<Limit> Limits { get; set; }
        public bool SingleRandomMatch { get; set; }

        // This is only relevant for stages with multiple patterns.
        // If this is true, the patterns will be chosen from greedily.
        // If this is false, the patterns will be cycled through instead.
        public bool Greedy { get; set; }

        // If this is true, the stage's input will be used as the regex, and the first
        // line of the stage will be used as the input.
        public bool InputAsRegex { get; set; }

        // If this is true, the matches and the separators between them 
        // will swap their roles.
        public bool InvertMatches { get; set; }

        // Custom regex modifiers

        // How to treat overlapping matches.
        public Overlaps Overlaps { get; set; }
        // How to treat repeated matches.
        public UniqueMatches UniqueMatches { get; set; }
        // Require the regex to cover the entire input.
        public Anchoring Anchoring { get; set; }
        // Make matches cyclic for $[ and $] in substitutions
        public bool CyclicMatches { get; set; }

        // General options which are interpreted differently by various stage types.
        public bool Reverse { get; set; }
        public bool Random { get; set; }
        public string ListStart { get; set; }
        public string ListSeparator { get; set; }
        public string ListEnd { get; set; }
        public string StringParam { get; set; }
        public Regex RegexParam { get; set; }

        // Configuration for Output stages
        public bool TrailingLinefeed { get; set; }
        public bool PrintOnlyIfChanged { get; set; }
        public bool PrePrint { get; set; }

        // Configuration for Split mode
        public bool OmitEmpty { get; set; }
        public bool OmitGroups { get; set; }

        // Configuration for Sort mode
        public bool SortNumerically { get; set; }
        
        // Configuration for Transliterate mode
        public bool CyclicTransliteration { get; set; }

        public Config()
        {
            Limits = new List<Limit>();

            ListStart = "";
            ListEnd = "";
            ListSeparator = "\n";
        }

        public void Inherit(Config other)
        {
            RegexOptions ^= other.RegexOptions;
        }

        public void Merge(Config other)
        {
            RegexOptions ^= other.RegexOptions;
            Limits.AddRange(other.Limits);
            Random |= other.Random;
            Reverse |= other.Reverse;
            if (RegexParam == null)
                RegexParam = other.RegexParam;
        }

        public Limit GetLimit(int i)
        {
            return i < Limits.Count ? Limits[i] : new Limit();
        }

        public int GetLimitCount()
        {
            return Limits.Count;
        }

        public string FormatAsList(IEnumerable<string> list)
        {
            var builder = new StringBuilder();
            builder.Append(ListStart);
            builder.Append(String.Join(ListSeparator, list));
            builder.Append(ListEnd);

            return builder.ToString();
        }
    }
}
