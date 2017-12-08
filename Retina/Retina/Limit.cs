using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina
{
    public class Limit
    {
        public bool Negated { get; set; }
        public int Begin { get; set; }
        public int End { get; set; }
        public int Step { get; set; }

        public Limit()
        {
            Begin = 0;
            End = -1;
            Step = 1;
        }

        public Limit(int n)
        {
            Begin = End = n;
            Step = 1;
        }

        public Limit(int m, int n)
        {
            Begin = m;
            End = n;
            Step = 1;
        }

        public Limit(int m, int k, int n)
        {
            Begin = m;
            End = n;
            Step = k;
        }

        public bool IsInRange(int value, int count)
        {
            int begin = Begin < 0 ? count + Begin : Begin;
            int end = End < 0 ? count + End : End;

            if (begin > end)
                return false;

            if (begin == end && value == begin)
                return true;

            int step = Step == 0 ? end - begin : Step;

            // This can only happen if begin == end.
            if (step == 0)
                step = 1;
            
            if (step > 0)
                return (value >= begin) && (value <= end) && ((value - begin) % step == 0);
            else
                return (value >= begin) && (value <= end) && ((end - value) % step == 0);
        }
    }
}
