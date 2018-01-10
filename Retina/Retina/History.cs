using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina
{
    public static class History
    {
        private static List<string> MostRecentResults;
        private static List<string> StageResults;

        public static void Initialize()
        {
            StageResults = new List<string>();
            StageResults.Add(null);
        }

        public static void Reset(string input)
        {
            MostRecentResults = new List<string>();
            MostRecentResults.Add(input);

            StageResults[0] = input;

            for (int i = 1; i < StageResults.Count; ++i)
                StageResults[i] = null;
        }

        // Returns an integer for the stage to use when sending in new results.
        public static int RegisterStage()
        {
            StageResults.Add(null);
            return StageResults.Count - 1;
        }

        public static void RegisterResult(int stage, string result)
        {
            MostRecentResults.Add(result);
            StageResults[stage] = result;
        }

        public static string GetMostRecentResult(int index)
        {
            if (index < MostRecentResults.Count)
                return MostRecentResults[MostRecentResults.Count - index - 1];
            else
                return null;
        }

        public static string GetStageResult(int index)
        {
            if (index < StageResults.Count)
                return StageResults[index];
            else
                return null;
        }
    }
}
