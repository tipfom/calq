using System.Collections.Generic;
using System.Linq;

namespace Calq.Core
{
    public abstract class Function : Term
    {
        public enum FuncType
        {
            Equals,
            Addition, Subtraction,
            Multiplication, Division,
            Power,

            Sqrt, Log, Sin, Cos, Differentiate,

            Lim, Int,
            Solve, Erf,

            Unknown
        }

        public bool IsInfix { get { return ((int)Name) < 6; } }

        public readonly FuncType Name;
        public readonly Term[] Parameters;

        protected Function(FuncType name)
        {
            Name = name;
        }
        protected abstract string GetStringRep();

        public override HashSet<string> GetVariableNames()
        {
            HashSet<string> ret = new HashSet<string>();

            foreach(Term t in Parameters)
                ret.UnionWith(t.GetVariableNames());

            return ret;
        }

        public override string ToPrefix()
        {
            return GetStringRep() + "[" + string.Join(",", Parameters.Select(x => x.ToString())) + "]";
        }

        public override string ToString()
        {
            if (IsInfix)
            {
                return string.Join(GetStringRep(), Parameters.Select(x => x.ToPrefix()));
            }
            else
            {
                return GetStringRep() + "(" + string.Join(",", Parameters.Select(x => x.ToPrefix())) + ")";
            }
        }
    }
}
