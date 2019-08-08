using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    class TermList : Term
    {
        public readonly Term[] Terms;

        public TermList(params Term[] terms) : base(TermType.TermList)
        {
            Terms = terms;
        }

        public static bool operator ==(TermList a, TermList b)
        {
            if (a.Terms.Length != b.Terms.Length) return false;

            for (int i = 0; i < a.Terms.Length; i++)
            {
                if (a.Terms[i] != b.Terms[i]) return false;
            }

            return true;
        }
        public static bool operator !=(TermList a, TermList b)
        {
            return !(a == b);
        }

        public override Term Approximate()
        {
            return new TermList(Terms.Select(x => x.Approximate()).ToArray());
        }

        public override Term Differentiate(string argument)
        {
            return new TermList(Terms.Select(x => x.Differentiate(argument)).ToArray());
        }

        public override Term Evaluate()
        {
            return new TermList(Terms.Select(x => x.Evaluate()).ToArray());
        }

        public override HashSet<string> GetVariableNames()
        {
            HashSet<string> ret = new HashSet<string>();

            foreach (Term t in Terms)
                ret.UnionWith(t.GetVariableNames());

            return ret;
        }

        public override string ToLaTeX()
        {
            return @"\left{" + string.Join(",", Terms.Select(x => x.ToLaTeX())) + @"\right}";
        }

        public override string ToPrefix()
        {
            return "{" + string.Join(",", Terms.Select(x => x.ToLaTeX())) + "}";
        }
    }
}
