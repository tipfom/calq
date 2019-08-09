using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Addition : Function
    {
        private HashSet<Term> inverseTerms = new HashSet<Term>();

        public Addition(params Term[] p) : base(FuncType.Addition, p) { }

        public bool IsInverse(Term t)
        {
            return inverseTerms.Contains(t);
        }

        public void MarkInverse(Term t)
        {
            inverseTerms.Add(t);
        }

        public override Term Reduce()
        {

            IEnumerable<(Term, bool)> reducedParameter = Parameters.Select(t => (t.Reduce(), IsInverse(t)));
            double addedValue = 0;
            foreach ((Term, bool) t in reducedParameter.Where(t => t.Item1.GetType() == typeof(Real)))
                addedValue += (t.Item2 ? -1 : 1) * ((Real)t.Item1).Value;

            List<(Term, bool)> remainingTerms = reducedParameter.Where(t => t.Item1.GetType() != typeof(Real)).ToList();
            if (addedValue != 0) remainingTerms.Add((new Real(addedValue), false));

            if (remainingTerms.Count == 0) return new Real(0);
            if (remainingTerms.Count == 1)
            {
                (Term, bool) remainingTerm = remainingTerms.First(x => true);
                if(remainingTerm.Item1.GetType() == typeof(Real))
                {
                    return new Real((remainingTerm.Item2 ? -1 : 1) * ((Real)remainingTerm.Item1).Value);
                }
                if(!remainingTerm.Item2)
                {
                    return remainingTerm.Item1;
                }
            }

            // nicht impl if (remainingTerms.Count == 1 ) return remainingTerms[0];

            Addition add = new Addition(remainingTerms.Select(t => t.Item1).ToArray());
            foreach ((Term, bool) t in remainingTerms) if (t.Item2) add.MarkInverse(t.Item1);
            return add;
        }

        public override Term Differentiate(string argument)
        {
            List<Term> diffTerms = new List<Term>();
            List<Term> invTerms = new List<Term>();
            foreach (Term t in Parameters)
            {
                Term d = t.Differentiate(argument);
                diffTerms.Add(d);
                if (IsInverse(t)) invTerms.Add(d);
            }

            Addition sum = new Addition(diffTerms.ToArray());
            foreach (Term t in invTerms)
                sum.MarkInverse(t);
            return sum;
        }

        //[TODO] zusammenfassen/vereinfachen
        public override Term Evaluate()
        {
            return this;
        }

        public override Term Approximate()
        {
            return this;
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

            append(Parameters[0], false);
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

        public override string ToString()
        {
            return ToPrefix();
        }

    }
}
