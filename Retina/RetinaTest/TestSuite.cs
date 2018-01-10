using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetinaTest
{
    // Thanks to Code Review:
    // https://codereview.stackexchange.com/a/166418/54789

    class TestSuite
    {
        public List<string> Sources { get; set; } = new List<string>();
        public TestCaseList TestCases { get; set; } = new TestCaseList();
    }

    class TestCase
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }

    class TestCaseList : List<TestCase>
    {
        public void Add(string input, string output) => Add(new TestCase { Input = input, Output = output });
    }
}
