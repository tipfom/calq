using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Addition : Function
    {
        private HashSet<Term> inverseTerms = new HashSet<Term>();

        public Addition(params Term[] p) : base(FuncType.Addition, p)
        {
        }

        public override Term Differentiate(string argument)
        {
            Term sum = Parameters[0].Differentiate(argument);
            for (int i = 1; i < Parameters.Length; i++)
                sum += Parameters[i].Differentiate(argument);
            return sum;
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum += Parameters[i].Evaluate();
            return sum;
        }
        public override Term Approximate()
        {
            Term sum = Parameters[0];

            for (int i = 1; i < Parameters.Length; i++)
                sum += Parameters[i].Approximate();
            return sum;
        }

        public override string ToLaTeX()
        {
            return ToString();
        }

        public override string ToPrefix()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("+[");
            AppendPrefixTermToString(Parameters[0], builder);
            for (int i = 1; i < Parameters.Length; i++)
            {
                builder.Append(",");
                AppendPrefixTermToString(Parameters[i], builder);
            }
            builder.Append("]");
            return builder.ToString();
        }

        public void MarkInverse(Term t)
        {
            inverseTerms.Add(t);
        }

        public bool IsInverse(Term t)
        {
            return inverseTerms.Contains(t);
        }

        private void AppendPrefixTermToString(Term t, StringBuilder builder)
        {
            if (IsInverse(t))
            {
                // TODO: hässlich, besser support mit nur einem Argument
                builder.Append("-[0," + t.ToPrefix() + "]");
            }
            else
            {

                builder.Append(t.ToPrefix());
            }
        }

        public override string ToString()
        {
            return ToPrefix();
        }
    }
}
