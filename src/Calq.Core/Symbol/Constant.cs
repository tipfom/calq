using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Constant : Symbol
    {
        public enum ConstType
        {
            Pi, E, I,
            Inf
        }

        public ConstType Name;

        public Constant(ConstType name) : base(SymbolType.Const)
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

        //[TODO] nur bei funktionen approximieren/wenn nötig
        public override Term Evaluate()
        {
            return this;
        }
        public override Term Differentiate(string argument)
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
            string sign = IsAddInverse ? "-" : "";
            switch (Name)
            {
                case ConstType.Pi: return sign + @"\pi";
                case ConstType.E: return sign + "e";
                case ConstType.Inf: return sign + @"\infty";
                case ConstType.I: return sign + "i";
            }

            return null;
        }

        public override string ToString()
        {
            string sign = IsAddInverse ? "-" : "";
            switch (Name)
            {
                case ConstType.Pi: return sign + "π";
                case ConstType.E: return sign + "e";
                case ConstType.Inf: return sign + "∞";
                case ConstType.I: return sign + "i";
            }

            return null;
        }
    }
}
