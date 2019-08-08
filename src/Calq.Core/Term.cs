using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public abstract class Term : IComparable<Term>
    {
        [Flags]
        public enum TermTag
        {
            None = 0,
            Inverse = 1,
        }

        public enum TermType
        {
            Symbol, Function, TermList, Vector
        }

        public TermType Type;
        public TermTag Tag;

        protected Term(TermType type)
        {
            Type = type;
        }

        public abstract Term Evaluate();
        public abstract Term Approximate();
        public abstract Term Differentiate(string argument);

        public abstract HashSet<string> GetVariableNames();

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

                    if (s[i] == ')')
                    {
                        if (brackets.Pop() != '(') return false;
                    }
                    else
                        if (brackets.Pop() != '{') return false;

                }
            }

            return brackets.Count == 0;
        }

        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }

        public static Term operator +(Term a, Term b)
        {
            List<Term> r = new List<Term>();

            Addition cast_a = a as Addition;
            if (cast_a != null)
                r.AddRange(cast_a.Parameters);
            else
                r.Add(a);

            Addition cast_b = b as Addition;
            if (cast_b != null)
                r.AddRange(cast_b.Parameters);
            else
                r.Add(b);

            // TODO performance
            return new Addition(r.ToArray());
        }

        public static Term operator -(Term a, Term b)
        {
            return a + -b;
        }

        public static Term operator *(Term a, Term b)
        {
            List<Term> r = new List<Term>();

            Multiplication cast_a = a as Multiplication;
            if (cast_a != null)
                r.AddRange(cast_a.Parameters);
            else
                r.Add(a);

            Multiplication cast_b = b as Multiplication;
            if (cast_b != null)
                r.AddRange(cast_b.Parameters);
            else
                r.Add(b);

            return new Multiplication(r.ToArray());
        }

        public static Term operator /(Term a, Term b)
        {
            if (b.Tag.HasFlag(TermTag.Inverse)) b.Tag &= ~TermTag.Inverse;
            else b.Tag |= TermTag.Inverse;
            return a * b;

        }

        public static Term operator -(Term a)
        {
            if (a.Tag.HasFlag(TermTag.Inverse)) a.Tag &= ~TermTag.Inverse;
            else a.Tag |= TermTag.Inverse;
            return a;
        }

        public static Term operator ^(Term a, Term b)
        {
            return new Power(a, b);
        }

        //public static Term operator -(Term a)
        //{
        //    return new Symbol(-1) * a;
        //}
        //public static Term operator -(Term a, Term b)
        //{
        //    return new Function(Function.Sub, a, b);
        //}
        //public static Term operator *(Term a, Term b)
        //{
        //    return new Function(Function.Mul, a, b);
        //}
        //public static Term operator /(Term a, Term b)
        //{
        //    return new Function(Function.Div, a, b);
        //}
        //public static Term operator ^(Term a, Term b)
        //{
        //    return new Function(Function.Pow, a, b);
        //}
    }
}
