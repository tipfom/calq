using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Symbolics;

namespace Calq.Core
{
    class TermList : Term
    {
        public List<Term> Terms;


        public TermList(List<Term> terms)
        {
            this.Terms = terms;
        }

        public TermList(string s)
        {
            Terms = new List<Term>();
            s = s.Substring(1, s.Length - 2);

            List<int> splits = new List<int>();
            int bracketDepth = 0;
            for(int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(' || s[i] == '{') bracketDepth++;
                if (s[i] == ')' || s[i] == '}') bracketDepth--;

                if (s[i] == ',' && bracketDepth == 0)
                {
                    splits.Add(i);
                }
            }

            splits.Insert(0, -1);
            splits.Add(s.Length);

            for (int j = 1; j < splits.Count; j++)
            {
                Terms.Add(TermFromMixedString(s.Substring(splits[j - 1] + 1, splits[j] - splits[j - 1] - 1)));
            }
            
        }

        public override Term Evaluate()
        {
            return this;
        }

        public override Expression GetAsExpression()
        {
            return Expression.Symbol(ToString());
        }

        public override string ToInfix()
        {
            return "{" + string.Join(",", Terms.Select(x => x.ToInfix())) + "}";
        }

        public override IEnumerable<string> GetVariableNames()
        {
            foreach (Term param in Terms)
            {
                foreach (string variable in param.GetVariableNames())
                    yield return variable;
            }
        }

        public override string ToString()
        {
            return "{" + string.Join(",", Terms) + "}";
        }

        public override Term Differentiate(string argument)
        {
            throw new NotImplementedException();
        }
        public override string ToLaTeX()
        {
            return @"\{" + string.Join(", ", Terms.Select(t => t.ToLaTeX())) + @"\}";
        }
    }
}
