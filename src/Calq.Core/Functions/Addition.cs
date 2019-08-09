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
            StringBuilder builder = new StringBuilder();
            void append(Term t, bool addPlus = true)
            {
                // TODO: hässlich, besser support mit nur einem Argument
                if (IsInverse(t)) builder.Append("-" + t.ToLaTeX());
                else builder.Append((addPlus ? "+" : "") + t.ToLaTeX());
            };

            append(Parameters[0],false);
            for (int i = 1; i < Parameters.Length; i++) { append(Parameters[i]); }
            return "(" + builder.ToString() + ")";
        }

        public override string ToPrefix()
        {
            StringBuilder builder = new StringBuilder();
            void append(Term t)
            {
                // TODO: hässlich, besser support mit nur einem Argument
                if (IsInverse(t)) builder.Append("-[0," + t.ToPrefix() + "]");
                else builder.Append(t.ToPrefix());
            };

            builder.Append("+[");
            append(Parameters[0]);
            for (int i = 1; i < Parameters.Length; i++)
            {
                builder.Append(",");
                append(Parameters[i]);
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

        public override string ToString()
        {
            return ToPrefix();
        }
    }
}
