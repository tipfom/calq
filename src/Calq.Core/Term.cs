using System;
using System.Collections.Generic;
using System.Linq;

namespace Calq.Core
{
    public abstract class Term : IComparable<Term>
    {
        public static IPythonProvider PlatformPythonProvider = new PythonWebProvider();

        public enum TermType
        {
            Symbol, Function, TermList, Vector
        }

        public readonly TermType Type;
        public bool IsAddInverse { get; set; } = false;
        public bool IsMulInverse { get; set; } = false;

        protected Term(TermType type, bool isAddInverse, bool isMulInverse)
        {
            Type = type;
            IsAddInverse = isAddInverse;
            IsMulInverse = isMulInverse;
        }

        public bool IsZero()
        {
            return GetType() == typeof(Real) && ((Real)this).Value == 1;
        }
        public bool IsOne()
        {
            return GetType() == typeof(Real) && ((Real)this).Value == 1;
        }
        public string GetSign()
        {
            return IsAddInverse ? "-" : "";
        }
        public string GetMultSign()
        {
            return IsMulInverse ? "/" : "";
        }
        public Term GetMultInverse()
        {
            Term b = Clone();
            b.IsMulInverse = !b.IsMulInverse;
            return b;
        }
        public override string ToString()
        {
            return ToInfix();
        }

        public abstract Term Evaluate();
        public abstract Term Approximate();
        public abstract Term GetDerivative(string argument);
        public abstract Term Reduce();
        public abstract Term MergeBranches();
        public abstract Term CheckAddReduce(Term t);
        public abstract HashSet<string> GetVariableNames();

        public abstract string ToInfix();
        public abstract string ToPrefix();

        public abstract string ToLaTeX();

        public abstract Term Clone();

        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }

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
        public static Term Parse(string s)
        {
            s = s.Replace(" ", "");
            if (s == "") throw new MissingArgumentException("InfixString was Empty");

            Term t = Function.InfixFuncFromString(s);

            if (!(t is null)) return t;

            t = Function.PrefixFuncFromString(s);
            if (!(t is null)) return t;

            if (s[0] == '{') return new TermList(s);

            if (s[0] == '(' && s[s.Length - 1] == ')')
                return Parse(s.Substring(1, s.Length - 2));

            return Symbol.FromString(s);
        }

        public static bool operator ==(Term a, Term b)
        {
            if (a is null || b is null)
                return a is null && b is null;
            if (a.Type != b.Type) return false;

            switch (a.Type)
            {
                case TermType.Symbol: return (Symbol)a == (Symbol)b;
                case TermType.Function: return (Function)a == (Function)b;
                case TermType.TermList: return (TermList)a == (TermList)b;
            }

            return false;
        }

        public static bool operator !=(Term a, Term b)
        {
            return !(a == b);
        }

        public static Addition operator +(Term a, Term b)
        {
            return new Addition(a.Clone(), b.Clone());
        }

        public static Addition operator -(Term a, Term b)
        {
            return a + -b;
        }
        public static Term operator -(Term a)
        {
            Term t = a.Clone();
            t.IsAddInverse = !t.IsAddInverse;
            return t;            
        }

        public static Multiplication operator *(Term a, Term b)
        {
            return new Multiplication(a.Clone(), b.Clone());
        }

        public static Multiplication operator /(Term a, Term b)
        {
            return a * b.GetMultInverse();
        }

        public static Power operator ^(Term a, Term b)
        {
            return new Power(a.Clone(), b.Clone());
        }

        public static implicit operator Term(double d) => new Real(d);
        public static implicit operator Term(int i) => new Real(i);
        public static implicit operator Term(string var) => new Variable(var);
    }
}
