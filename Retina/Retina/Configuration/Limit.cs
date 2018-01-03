namespace Retina.Configuration
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
            bool result;

            int begin = Begin < 0 ? count + Begin : Begin;
            int end = End < 0 ? count + End : End;

            if (begin > end)
                result = false;
            else if (begin == end && value == begin)
                result = true;
            else
            {
                int step = Step == 0 ? end - begin : Step;

                // This can only happen if begin == end.
                if (step == 0)
                    step = 1;

                if (step > 0)
                    result = (value >= begin) && (value <= end) && ((value - begin) % step == 0);
                else
                    result = (value >= begin) && (value <= end) && ((end - value) % step == 0);
            }

            // Invert the result if Negated is true.
            return Negated ^ result;
        }
    }
}
