using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina.Configuration
{
    public enum UniqueMatches
    {
        Off = 0,
        // If the same substring is matched multiple times, discard the new one.
        KeepFirst,
        // If the same substring is matched multiple times, discard the old one.
        KeepLast,
    }
}
