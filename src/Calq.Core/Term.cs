using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public abstract class Term
    {
        public abstract Term Evaluate();
        public abstract Term Approximate();

        public static Term TermFromMixedString(string s)
        {
            Term x = Function.FunctionFromMixedString(s);
            if (x == null)
                return new Variable(s);
            else
                return x;
        }
    }
}
