using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Symbolics;

namespace Calq.Core
{
    public abstract class Term
    {
        public abstract Expression Evaluate();

        public abstract IEnumerable<string> GetVariableNames();

        public static Term TermFromMixedString(string s)
        {
            s = s.Replace(" ", "");

            Term x = Function.FunctionFromMixedString(s);
            if (x == null)
                return new Variable(s);
            else
                return x;
        }
    }
}
