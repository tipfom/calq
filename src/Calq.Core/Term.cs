using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public abstract class Term
    {
        public abstract Term Evaluate();
        public abstract HashSet<string> GetVariableNames();
        public abstract Term Differentiate(string argument);

        public abstract string ToPrefix();
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

        public static Term operator +(Term a, Term b)
        {
            return new Function(Function.Add, a, b);
        }
        public static Term operator -(Term a)
        {
            return new Variable(-1) * a;
        }
        public static Term operator -(Term a, Term b)
        {
            return new Function(Function.Sub, a, b);
        }
        public static Term operator *(Term a, Term b)
        {
            return new Function(Function.Mul, a, b);
        }
        public static Term operator /(Term a, Term b)
        {
            return new Function(Function.Div, a, b);
        }
        public static Term operator ^(Term a, Term b)
        {
            return new Function(Function.Pow, a, b);
        }
    }
}
