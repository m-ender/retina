using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retina
{
    public class History
    {
        private List<string> ResultLog;
        private List<string> StageResults;
        private bool LogActive;
        private int LogLimit;

        public History()
        {
            LogActive = false;
            LogLimit = -1;
            ResultLog = new List<string>();
            StageResults = new List<string>();
            StageResults.Add(null);
        }

        public void ActivateLog()
        {
            LogActive = true;
        }

        // Returns an integer for the stage to use when sending in new results.
        public int RegisterStage()
        {
            StageResults.Add(null);
            return StageResults.Count - 1;
        }

        public void RegisterResult(int stage, string result)
        {
            if (LogActive && LogLimit != 0)
            {
                while (LogLimit > 0 && ResultLog.Count >= LogLimit)
                    ResultLog.RemoveAt(0);
                ResultLog.Add(result);
            }
            StageResults[stage] = result;
        }

        public string GetMostRecentResult(int index)
        {
            if (index < ResultLog.Count)
                return ResultLog[ResultLog.Count - index - 1];
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

        public void LimitLog(int logLimit)
        {
            LogLimit = logLimit;
        }
    }
}
