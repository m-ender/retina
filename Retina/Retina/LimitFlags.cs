using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina
{
    [Flags]
    public enum LimitFlags
    {
        None =    0,
        Less =    1,
        Equals =  2,
        Greater = 4,
    }
}
