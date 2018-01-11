using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina
{
    public class History
    {
        private List<string> MostRecentResults;
        private List<string> StageResults;

        public History()
        {
            MostRecentResults = new List<string>();
            StageResults = new List<string>();
            StageResults.Add(null);
        }

        // Returns an integer for the stage to use when sending in new results.
        public int RegisterStage()
        {
            StageResults.Add(null);
            return StageResults.Count - 1;
        }

        public void RegisterResult(int stage, string result)
        {
            MostRecentResults.Add(result);
            StageResults[stage] = result;
        }

        public string GetMostRecentResult(int index)
        {
            if (index < MostRecentResults.Count)
                return MostRecentResults[MostRecentResults.Count - index - 1];
            else
                return null;
        }

        public string GetStageResult(int index)
        {
            if (index < StageResults.Count)
                return StageResults[index];
            else
                return null;
        }
    }
}
