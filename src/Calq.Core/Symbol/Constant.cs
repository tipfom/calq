using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Constant : Symbol, IComparable<Constant>
    {
        public enum ConstType
        {
            Pi, E, I,
            Inf
        }

        public ConstType Name;

        public Constant(ConstType name) : base(SymbolType.Const, false, false)
        {
            Name = name;
        }
        public Constant(ConstType name, bool isAddInverse, bool isMultInverse) : base(SymbolType.Const, isAddInverse, isMultInverse)
        {
            Name = name;
        }

        public static Constant FromName(string name)
        {
            switch (name)
            {
                case "pi": return new Constant(ConstType.Pi);
                case "e": return new Constant(ConstType.E);
                case "i": return new Constant(ConstType.I);
                case "inf": return new Constant(ConstType.Inf);
            }

            return null;
        }

        public static bool operator ==(Constant a, Constant b)
        {
            return a.Name == b.Name;
        }
        public static bool operator !=(Constant a, Constant b)
        {
            return !(a == b);
        }

        public override Term Clone()
        {
            return new Constant(Name, IsAddInverse, IsMulInverse);
        }

        public override Term Evaluate()
        {
            return this;
        }
        public override Term GetDerivative(string argument)
        {
            return new Real(0);
        }

        public override Term Approximate()
        {
            switch (Name)
            {
                case ConstType.Pi: return new Real(3.141);
                case ConstType.E: return new Real(2.718);
                case ConstType.Inf: return new Real(double.PositiveInfinity);
                default: return Evaluate();
            }
        }

        public override HashSet<string> GetVariableNames()
        {
            return new HashSet<string>();
        }

        public override string ToLaTeX()
        {
            string sign = GetSign();
            switch (Name)
            {
                case ConstType.Pi: return sign + @"\pi";
                case ConstType.E: return sign + "e";
                case ConstType.Inf: return sign + @"\infty";
                case ConstType.I: return sign + "i";
            }

            return null;
        }

        public override string ToInfix()
        {
            string sign = GetSign();
            switch (Name)
            {
                case ConstType.Pi: return sign + "pi";
                case ConstType.E: return sign + "e";
                case ConstType.Inf: return sign + "infty";
                case ConstType.I: return sign + "i";
            }

            return "error";
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Term CheckAddReduce(Term t)
        {
            if (t.GetType() == typeof(Constant))
            {
                if (((Constant)t).Name == this.Name)
                {
                    if (t.IsAddInverse && IsAddInverse) return 2 * t;
                    if (t.IsAddInverse || IsAddInverse) return 0;
                    return 2 * t;
                }
            }
            return null;
        }

        public int CompareTo(Constant other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
