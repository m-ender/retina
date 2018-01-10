using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetinaTest
{
    class RandomTestSuite
    {
        public List<string> Sources { get; set; } = new List<string>();
        public RandomTestCaseList TestCases { get; set; } = new RandomTestCaseList();
    }

    class RandomTestCase
    {
        public string Input { get; set; }
        // Pairs each possible output with a probability of its occurrence.
        public List<Tuple<string, double>> Outputs { get; set; }
    }

    class RandomTestCaseList : List<RandomTestCase>
    {
        public void Add(string input, string[] outputs)
        {
            double probability = 1.0 / outputs.Length;
            Add(new RandomTestCase
            {
                Input = input,
                Outputs = outputs.Select(s => new Tuple<string, double>(s, probability)).ToList()
            });
        }
        public void Add(string input, List<Tuple<string, double>> outputs) => Add(new RandomTestCase { Input = input, Outputs = outputs });
    }
}
