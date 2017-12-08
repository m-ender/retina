using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Retina
{
    public class Configuration
    {
        public RegexOptions RegexOptions { get; set; }

        // General configuration        
        public List<Limit> Limits { get; set; }

        // Configuration for Output stages
        public bool TrailingLinefeed { get; set; }
        public bool PrintOnlyIfChanged { get; set; }

        // Configuration for Match mode
        public bool Overlapping { get; set; }
        public bool PrintMatches { get; set; }
        public bool Unique { get; set; }

        // Configuration for Split mode
        public bool OmitEmpty { get; set; }
        public bool OmitGroups { get; set; }

        // Configuration for Replace mode

        // Configuration for Sort mode
        public bool SortNumerically { get; set; }
        public bool SortReverse { get; set; }

        // Configuration for Sort and Deduplicate mode
        public bool UseSubstitution { get; set; }

        public Configuration()
        {
            Limits = new List<Limit>();
        }

        public void Inherit(Configuration other)
        {
            RegexOptions ^= other.RegexOptions;
        }

        public void Merge(Configuration other)
        {
            RegexOptions ^= other.RegexOptions;
            Limits.AddRange(other.Limits);
        }

        public Limit GetLimit(int i)
        {
            return i < Limits.Count ? Limits[i] : new Limit();
        }
    }
}
