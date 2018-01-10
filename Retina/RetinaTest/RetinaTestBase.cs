using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retina;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RetinaTest
{
    [TestClass]
    public class RetinaTestBase
    {
        internal void AssertProgram(TestSuite testSuite)
        {
            var interpreter = new Interpreter(testSuite.Sources);

            foreach (var testCase in testSuite.TestCases)
            {
                var actualOutput = new StringWriter();
                interpreter.Execute(testCase.Input, actualOutput);

                Assert.AreEqual(testCase.Output, actualOutput.ToString());
            }
        }

        static readonly double[] chiSquaredCriticalValues = {
            0.0, 3.841, 5.991, 7.815, 9.488, 11.070, 12.592, 14.067,
            15.507, 16.919, 18.307, 19.675, 21.026, 22.362, 23.685,
            24.996, 26.296, 27.587, 28.869, 30.144, 31.410, 32.671,
            33.924, 35.172, 36.415, 37.652, 38.885, 40.113, 41.337,
            42.557, 43.773, 44.985, 46.194, 47.400, 48.602, 49.802,
            50.998, 52.192, 53.384, 54.572, 55.758, 56.942, 58.124,
            59.304, 60.481, 61.656, 62.830, 64.001, 65.171, 66.339,
            67.505, 68.669, 69.832, 70.993, 72.153, 73.311, 74.468,
            75.624, 76.778, 77.931, 79.082, 80.232, 81.381, 82.529,
            83.675, 84.821, 85.965, 87.108, 88.250, 89.391, 90.531,
            91.670, 92.808, 93.945, 95.081, 96.217, 97.351, 98.484,
            99.617, 100.749, 101.879, 103.010, 104.139, 105.267,
            106.395, 107.522, 108.648, 109.773, 110.898, 112.022,
            113.145, 114.268, 115.390, 116.511, 117.632, 118.752,
            119.871, 120.990, 122.108, 123.225, 124.342
        };

        internal void AssertRandomProgram(RandomTestSuite testSuite)
        {
            var interpreter = new Interpreter(testSuite.Sources);

            foreach (var testCase in testSuite.TestCases)
            {
                int nOutputs = testCase.Outputs.Count;
                int samples = 240;

                var expectedOutcomes = new Dictionary<string, int>();
                var outcomes = new Dictionary<string, int>();
                int expectedRemainder = samples;
                int remainder = samples;
                testCase.Outputs.ForEach(outcome => {
                    int expectedSamples = (int)(outcome.Item2 * samples);
                    remainder -= expectedSamples;
                    expectedRemainder -= expectedSamples;
                    outcomes[outcome.Item1] = expectedSamples;
                    expectedOutcomes[outcome.Item1] = expectedSamples;
                });
                
                for (int i = 0; i < samples; ++i)
                {
                    var actualOutput = new StringWriter();
                    interpreter.Execute(testCase.Input, actualOutput);
                    var actualString = actualOutput.ToString();
                    if (outcomes.ContainsKey(actualString))
                        --outcomes[actualString];
                    else
                        --remainder;
                }

                double chiSquared = 0;
                //if (expectedRemainder > 0)
                //    chiSquared += remainder * remainder / expectedRemainder;

                foreach (var outcome in outcomes)
                    chiSquared += outcome.Value * outcome.Value / expectedOutcomes[outcome.Key];

                Assert.IsTrue(chiSquared <= chiSquaredCriticalValues[nOutputs-1]);
            }
        }
    }
}
