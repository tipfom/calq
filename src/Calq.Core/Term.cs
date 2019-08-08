using System;
using System.Collections.Generic;
using System.Linq;

namespace Calq.Core
{
    public abstract class Term : IComparable<Term>
    {
        public enum TermType
        {
            Symbol, Function, TermList, Vector
        }

        public TermType Type;

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

            // TODO: Performance
            Addition add = new Addition(r.ToArray());
            if (cast_a != null) foreach (Term t in cast_a.Parameters) if (cast_a.IsInverse(t)) add.MarkInverse(t);
            if (cast_b != null) foreach (Term t in cast_b.Parameters) if (cast_b.IsInverse(t)) add.MarkInverse(t);

            return add;
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

            Multiplication mult = new Multiplication(r.ToArray());
            if (cast_a != null) foreach (Term t in cast_a.Parameters) if (cast_a.IsInverse(t)) mult.MarkInverse(t);
            if (cast_b != null) foreach (Term t in cast_b.Parameters) if (cast_b.IsInverse(t)) mult.MarkInverse(t);

            return mult;
        }

        public static Term operator /(Term a, Term b)
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

            Multiplication mult = new Multiplication(r.ToArray());
            if (cast_a != null) { foreach (Term t in cast_a.Parameters) if (cast_a.IsInverse(t)) mult.MarkInverse(t); }
            if (cast_b != null) { foreach (Term t in cast_b.Parameters) if (!cast_b.IsInverse(t)) mult.MarkInverse(t); }
            else mult.MarkInverse(b);

            return mult;
        }

        public static Term operator -(Term a)
        {
            Addition cast_a = a as Addition;
            if (cast_a != null)
            {
                Addition add = new Addition(cast_a.Parameters);
                if (cast_a != null) { foreach (Term t in cast_a.Parameters) if (!cast_a.IsInverse(t)) add.MarkInverse(t); }
                return add;
            }
            else
            {
                Addition add = new Addition(a);
                add.MarkInverse(a);
                return add;
            }
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
