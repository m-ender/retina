using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina.Configuration
{
    public enum Anchoring
    {
        None = 0,
        // Anchor the match to the entire string.
        String,
        // Anchor the match to line endings.
        Line,
    }
}
