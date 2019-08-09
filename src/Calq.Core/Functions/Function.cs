using System.Collections.Generic;

namespace Calq.Core
{
    public abstract class Function : Term
    {
        public enum FuncType
        {
            Equals,
            Addition, 
            Multiplication, 
            Power,

            Sqrt, Log, Sin, Cos, Differentiate,

            Lim, Int,
            Solve, Erf,

            Unknown
        }

        public readonly FuncType Name;
        public readonly Term[] Parameters;
        protected Function(FuncType name, Term[] paras) : base(TermType.Function)
        {
            Name = name;
            Parameters = paras;
        }

        //[TODO] Reihenfolge checken
        public static bool operator ==(Function a, Function b)
        {
            if ((object)a == null || (object)a == null)
                return (object)a == null && (object)a == null;

            if (a.Name != b.Name) return false;
            if (a.Parameters.Length != b.Parameters.Length) return false;

            for(int i = 0; i < a.Parameters.Length; i++)
            {
                if (a.Parameters[i] != b.Parameters[i]) return false;
            }

            return true;
        }
        public static bool operator !=(Function a, Function b)
        {
            return !(a == b);
        }

        public override HashSet<string> GetVariableNames()
        {
            HashSet<string> ret = new HashSet<string>();

            foreach(Term t in Parameters)
                ret.UnionWith(t.GetVariableNames());

            return ret;
        }
       
    }
}
