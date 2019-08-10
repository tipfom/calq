﻿using System;
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
            List<Term> paras = Parameters.Select(x => x.Reduce()).Where(x => !x.IsOne()).ToList();

            List<Real> reals = paras.Where(x => x.GetType() == typeof(Real)).Cast<Real>().ToList();
            if (reals.Count > 1)
            {
                double product = 1;
                for (int i = 0; i < reals.Count; i++)
                {
                    if (reals[i].IsAddInverse)
                        product /= reals[i].Value;
                    else
                        product *= reals[i].Value;
                }

                paras = paras.Where(x => x.GetType() != typeof(Real)).ToList();
                if (product != 1) paras.Add(product);
            }

            switch (paras.Count)
            {
                case 0: return 0;
                case 1: return paras[0];
                default: return new Multiplication(IsAddInverse, IsMulInverse, paras.ToArray());
            }
        }
    }
}
