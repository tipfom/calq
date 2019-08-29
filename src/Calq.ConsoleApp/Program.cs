using Calq.Core;
using System;
using System.Collections.Generic;

namespace Calq.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = Console.ReadLine();

            while(s != "stop")
            {
                Term t = Term.Parse(s);
                string expLat = t.ToLaTeX();

                t = t.MergeBranches();
                t = t.Evaluate();

                int i = 0;
                int lastLength = int.MaxValue;
                while (t.ToInfix().Length < lastLength)
                {
                    lastLength = t.ToInfix().Length;
                    t = t.MergeBranches();
                    t = t.Reduce();
                    i++;

                    Console.WriteLine("= " + t.ToInfix());
                }

                s = Console.ReadLine();
            }
        }
    }
}
