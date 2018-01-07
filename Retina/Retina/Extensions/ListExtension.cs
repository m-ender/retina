using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retina.Extensions
{
    public static class ListExtension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.RNG.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
