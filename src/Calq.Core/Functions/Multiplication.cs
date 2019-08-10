using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Multiplication : Function
    {
        public Multiplication(params Term[] p) : base(FuncType.Multiplication, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");
        }
        public Multiplication(bool isAddInverse, bool isMultInverse, params Term[] p) : base(FuncType.Multiplication, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");

            IsAddInverse = isAddInverse;
            IsMulInverse = isMultInverse;
        }

        public override Term Differentiate(string argument)
        {
            List<Term> sums = new List<Term>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Term r = Parameters[i];

                if (r.IsMulInverse)
                    r = r.Differentiate(argument) / (r ^ 2);
                else
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
            List<Term> inverse = Parameters.Where(t => t.IsMulInverse).ToList();

            if (inverse.Count > 0)
            {
                return $@"\frac{"{" + string.Join(@"\cdot ", Parameters.Where(t => !t.IsMulInverse).Select(t => t.ToLaTeX())) + "}"}{"{" + string.Join(@"\cdot ", inverse.Select(t => t.ToLaTeX())) + "}"}";
            }
            else
            {
                return "(" + string.Join(@"\cdot ", Parameters.Select(t => t.ToLaTeX())) + ")";
            }
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
            if (t.IsMulInverse)
            {
                // TODO: hässlich, besser support mit nur einem Argument
                builder.Append("/[1," + t.ToPrefix() + "]");
            }
            else
            {

                builder.Append(t.ToPrefix());
            }
        }

        public override Term Reduce()
        {
            // TODO help
            var x = Parameters.FirstOrDefault(t => t.Reduce().Equals(new Constant(Constant.ConstType.Inf)));
            if (x != null)
            {
                if (x.IsMulInverse)
                {
                    return new Real(0);
                }
                return new Constant(Constant.ConstType.Inf);
            }

            IEnumerable<Term> reducedParameter = Parameters.Select(t => t.Reduce());
            double addedValue = 1;
            foreach (Term t in reducedParameter.Where(t => t.GetType() == typeof(Real)))
            {
                Real r = (Real)t;
                if (r.Value == 0)
                {
                    if (t.IsMulInverse) return new Constant(Constant.ConstType.Inf);
                    return new Real(0);
                }

                if (t.IsMulInverse)
                {
                    addedValue /= ((Real)t).Value;
                }
                else
                {
                    addedValue *= ((Real)t).Value;
                }
            }


            List<Term> remainingTerms = reducedParameter.Where(t => t.GetType() != typeof(Real)).ToList();
            if (addedValue != 0) remainingTerms.Add(new Real(addedValue));

            if (remainingTerms.Count == 1) return remainingTerms[0];

            Multiplication mul = new Multiplication(remainingTerms.ToArray());
            
            return mul;
        }
    }
}
