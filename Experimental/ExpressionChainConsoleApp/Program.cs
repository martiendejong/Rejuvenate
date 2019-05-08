using ExpressionChain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionChainConsoleApp
{
    class Program
    {
        public class yo
        {

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var source = new List<object>() { "a", "b" }.AsQueryable();

            var x = new ChainLinqQueryable<object>(null, source);
            var y  = x.Where(o => true).Select(o => new yo());
            var x1 = y._expression.Parent;
        }
    }
}
