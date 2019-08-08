using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Multiplication : Function
    {
        private HashSet<Term> inverseTerms = new HashSet<Term>();

        public Multiplication(params Term[] p) : base(FuncType.Multiplication, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");
        }

        public override Term Differentiate(string argument)
        {
            List<Term> sums = new List<Term>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Term r = Parameters[i];
                if (IsInverse(r))
                {
                    r = r ^ (-new Real(1));
                }
                r = r.Differentiate(argument);
                for (int j = 0; j < Parameters.Length; j++)
                {
                    if (i == j) continue;
                    r *= Parameters[j];
                }
                sums.Add(r);
            }
            return new Addition(sums.ToArray());
        }

        public void MarkInverse(Term t)
        {
            inverseTerms.Add(t);
        }

        public bool IsInverse(Term t)
        {
            return inverseTerms.Contains(t);
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum *= Parameters[i].Evaluate();
            return sum;
        }
        public override Term Approximate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum *= Parameters[i].Approximate();
            return sum;
        }

        public override string ToLaTeX()
        {
            return ToString();
        }

        public override string ToString()
        {
            return ToPrefix();
        }

        public override string ToPrefix()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("*[");
            AppendPrefixTermToString(Parameters[0], builder);
            for (int i = 1; i < Parameters.Length; i++)
            {
                builder.Append(",");
                AppendPrefixTermToString(Parameters[i], builder);
            }
            builder.Append("]");
            return builder.ToString();
        }

        private void AppendPrefixTermToString(Term t, StringBuilder builder)
        {
            if (IsInverse(t))
            {
                // TODO: hässlich, besser support mit nur einem Argument
                builder.Append("/[1," + t.ToPrefix() + "]");
            }
            else
            {

                builder.Append(t.ToPrefix());
            }
        }
    }
}
