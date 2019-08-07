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

        public static bool CheckBracketCount(string s)
        {
            int bracketDepth = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(') bracketDepth++;
                if (s[i] == ')') bracketDepth--;

                if (bracketDepth < 0) return false;
            }

            return bracketDepth == 0;
        }
        public static Term TermFromMixedString(string s)
        {

            if (s == null)
                throw new MissingArgumentException("");
            s = s.Replace(" ", "");
            if(s == "")
                throw new MissingArgumentException("");

            Term x = Function.FunctionFromMixedString(s);
            if (x == null)
                return new Variable(s);
            else
                return x;
        }
    }
}
