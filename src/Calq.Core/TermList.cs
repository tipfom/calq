using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Symbolics;

namespace Calq.Core
{
    class TermList : Term
    {
        List<Term> terms;


        public TermList(List<Term> terms)
        {
            this.terms = terms;
        }

        public TermList(string s)
        {
            terms = new List<Term>();
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
                terms.Add(TermFromMixedString(s.Substring(splits[j - 1] + 1, splits[j] - splits[j - 1] - 1)));
            }
            
        }

        public override Expression Evaluate()
        {
            throw new NotEvaluateableException("Lists cant be evaluated (yet)");
        }

        public override Expression GetAsExpression()
        {
            return Expression.Symbol(ToString());
        }

        public override string GetInfix()
        {
            return "{" + string.Join(",", terms.Select(x => x.GetInfix())) + "}";
        }

        public override IEnumerable<string> GetVariableNames()
        {
            foreach (Term param in terms)
            {
                foreach (string variable in param.GetVariableNames())
                    yield return variable;
            }
        }

        public override string ToString()
        {
            return "{" + string.Join(",", terms) + "}";
        }
    }
}
