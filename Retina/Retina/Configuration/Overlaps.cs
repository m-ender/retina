using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina.Configuration
{
    public enum Overlaps
    {
        None = 0,
        // Find up to one match per starting-position, even if they overlap.
        OverlappingSimple,
        // Find all substrings that can be matched, even if they overlap.
        OverlappingAll,
    }
}
