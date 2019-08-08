using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Symbolics;

namespace Calq.Core
{
    public abstract class Term
    {
        public abstract Term Evaluate();
        public abstract Expression GetAsExpression();
        public abstract string ToInfix();
        public abstract IEnumerable<string> GetVariableNames();
        public abstract Term Differentiate(string argument);

        public abstract string ToLaTeX();
        public static bool CheckBracketCount(string s)
        {
            if (s == null)
                throw new MissingArgumentException("");
            s = s.Replace(" ", "");
            if (s == "")
                throw new MissingArgumentException("");

            Stack<char> brackets = new Stack<char>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(' || s[i] == '{') brackets.Push(s[i]);

                if (s[i] == ')' || s[i] == '}')
                {
                    if (brackets.Count == 0) return false;

                    if(s[i] == ')')
                    {
                        if (brackets.Pop() != '(') return false;
                    }
                    else
                        if (brackets.Pop() != '{') return false;

                }
            }

            return brackets.Count == 0;
        }
        public static Term TermFromMixedString(string s)
        {
            if (s == null)
                throw new MissingArgumentException("");
            s = s.Replace(" ", "");
            if(s == "")
                throw new MissingArgumentException("");
            

            Term x = Function.FunctionFromMixedString(s);
            if (x != null)
                return x;
            else
            {
                if(s[0] == '{' && s[s.Length - 1] == '}')
                {
                    return new TermList(s);
                }
                else return new Variable(s);
            }
        }
    }
}
