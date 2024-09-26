using System;

namespace Septem.Utils.Helpers.CodeGeneration
{
    public class RandomCodeGenerator : IRandomCodeGenerator
    {
        private readonly Random _rnd;

        public RandomCodeGenerator()
        {
            _rnd = new Random(System.DateTime.Now.Ticks.GetHashCode());
        }

        public string Generate(int length)
        {
            var end = (int)Math.Pow(10, length) - 1;
            var start = end / 10 + 1;
            return _rnd.Next(start, end).ToString();
        }
    }
}
