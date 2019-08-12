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
        public TermList(bool isAddInverse, bool isMulInverse, params Term[] terms) : base(TermType.TermList)
        {
            Terms = terms;
            IsAddInverse = isAddInverse;
            IsMulInverse = isMulInverse;
        }

        public TermList(string s) : base(TermType.TermList)
        {
            s = s.Substring(1, s.Length - 2);

            List<int> splits = new List<int>();
            int bracketDepth = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(' || s[i] == '{') bracketDepth++;
                if (s[i] == ')' || s[i] == '}') bracketDepth--;

                if (s[i] == ',' && bracketDepth == 1)
                {
                    splits.Add(i);
                }
            }

            Terms = new Term[splits.Count + 1];

            splits.Insert(0, -1);
            splits.Add(s.Length);

            for (int j = 1; j < splits.Count; j++)
            {
                Terms[j - 1] = Parse(s.Substring(splits[j - 1] + 1, splits[j] - splits[j - 1] - 1));
            }
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

        public override Term GetDerivative(string argument)
        {
            return new TermList(Terms.Select(x => x.GetDerivative(argument)).ToArray());
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

        public override Term Reduce()
        {
            throw new NotImplementedException();
        }

        public override string ToLaTeX()
        {
            return GetSign() + @"\left{" + string.Join(",", Terms.Select(x => x.ToLaTeX())) + @"\right}";
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override string ToPrefix()
        {
            return GetSign() + "{" + string.Join(",", Terms.Select(x => x.ToPrefix())) + "}";
        }

        public override Term Clone()
        {
            return new TermList(Terms.Select(x => x.Clone()).ToArray());
        }

        public override Term MergeBranches()
        {
            return new TermList(Terms.Select(x => x.MergeBranches()).ToArray());
        }

        public override string ToInfix()
        {
            return GetSign() + "{" + string.Join(",", Terms.Select(x => x.ToInfix())) + "}";
        }
    }
}
