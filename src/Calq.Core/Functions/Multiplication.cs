using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Multiplication : Function
    {
        public Multiplication(params Term[] p) : base(FuncType.Multiplication, false, false, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");
        }
        public Multiplication(bool isAddInverse, bool isMultInverse, params Term[] p) : base(FuncType.Multiplication, isAddInverse, isMultInverse, p)
        {
            if (p.Length < 2)
                throw new InvalidParameterCountException("Multiplication needs at least 2 arguments");
        }

        public override Term GetDerivative(string argument)
        {
            List<Term> sums = new List<Term>();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Term r = Parameters[i];

                if (r.IsMulInverse)
                    r = r.GetDerivative(argument) / (r ^ 2);
                else
                    r = r.GetDerivative(argument);

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
                List<Term> normal = Parameters.Where(t => !t.IsMulInverse).ToList();

                if (normal.Count == 0)
                    return GetSign() + $@"\frac{"{1}"}{"{" + string.Join(@"\cdot ", inverse.Select(t => t.ToLaTeX())) + "}"}";
                else
                    return GetSign() + $@"\frac{"{" + string.Join(@"\cdot ", normal.Select(t => t.ToLaTeX())) + "}"}{"{" + string.Join(@"\cdot ", inverse.Select(t => t.ToLaTeX())) + "}"}";
            }
            else
            {
                return GetSign() + "(" + string.Join(@"\cdot ", Parameters.Select(t => t.ToLaTeX())) + ")";
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Term Reduce()
        {
            List<Term> paras = Parameters.Select(x => x.Reduce()).Where(x => !x.IsOne()).ToList();

            if (paras.Where(x => x.IsZero()).Count() > 0)
                return 0;

            // merge branches
            for (int i = 0; i < paras.Count; i++)
            {
                if (paras[i].GetType() == typeof(Multiplication))
                {
                    paras.AddRange(((Multiplication)paras[i]).Parameters);
                    paras.RemoveAt(i);
                }
            }

            // extract sign
            bool isAddInverse = IsAddInverse;
            foreach (Term t in paras)
                if (t.IsAddInverse) isAddInverse = !isAddInverse;

            // merge powers
            List<(Term b, Term p)> powerCounter = new List<(Term, Term)>();
            for (int i = 0; i < paras.Count; i++)
            {
                if (paras[i].GetType() == typeof(Real)) continue;

                if (paras[i].GetType() == typeof(Power))
                {
                    Power p = paras[i] as Power;
                    int index = powerCounter.FindIndex(x => x.b == p.Parameters[0]);
                    if (index > -1)
                    {
                        if (paras[i].IsMulInverse)
                            powerCounter[index] = (powerCounter[index].b, powerCounter[index].p - p.Parameters[1]);
                        else
                            powerCounter[index] = (powerCounter[index].b, powerCounter[index].p + p.Parameters[1]);

                    }
                    else
                    {
                        if (p.IsMulInverse)
                            powerCounter.Add((p.Parameters[0], -p.Parameters[1]));
                        else
                            powerCounter.Add((p.Parameters[0], p.Parameters[1]));
                    }
                }
                else
                {
                    int index = powerCounter.FindIndex(x => x.b == paras[i]);
                    if (index > -1)
                    {
                        if (paras[i].IsMulInverse)
                            powerCounter[index] = (powerCounter[index].b, powerCounter[index].p - new Real(1));
                        else
                            powerCounter[index] = (powerCounter[index].b, powerCounter[index].p + new Real(1));

                    }
                    else
                    {
                        if (paras[i].IsMulInverse)
                            powerCounter.Add((paras[i], -new Real(1)));
                        else
                            powerCounter.Add((paras[i], new Real(1)));
                    }
                }
            }

            double real = 1;
            foreach (Real r in paras.Where(x => x.GetType() == typeof(Real)).Cast<Real>())
            {
                if (r.IsMulInverse)
                    real /= r.Value;
                else
                    real *= r.Value;
            }
            if (real != 1) powerCounter.Add((new Real(real), 1));

            List<Term> reducedTerms = new List<Term>();
            foreach ((Term b, Term p) powerPair in powerCounter)
            {
                Term p = powerPair.p.Reduce();
                Term b = powerPair.b.Clone();
                if (p.IsOne())
                {
                    b.IsAddInverse = false;
                    b.IsMulInverse = p.IsAddInverse;
                    reducedTerms.Add(b);
                }
                else
                {
                    b.IsMulInverse = false;
                    b.IsAddInverse = false;
                    if (p.IsAddInverse)
                    {
                        reducedTerms.Add(new Power(false, true, b, p));
                    }
                    else
                    {
                        reducedTerms.Add(new Power(false, false, b, p));
                    }
                }
            }

            switch (reducedTerms.Count)
            {
                case 0: return 1;
                case 1:
                    if (reducedTerms[0].IsMulInverse)
                        return new Multiplication(IsAddInverse, IsMulInverse, 1, reducedTerms[0]);
                    else
                        return reducedTerms[0];
                default: return new Multiplication(IsAddInverse, IsMulInverse, reducedTerms.ToArray());
            }
        }

        public override Term CheckAddReduce(Term t)
        {
            if (t.GetType() == typeof(Multiplication))
            {
                Multiplication cMult = (Multiplication)t;

                List<Term> a = Parameters.ToList(), b = cMult.Parameters.ToList();
                List<Term> commonTerms = new List<Term>();

                for (int i = 0; i < a.Count; i++)
                {
                    for (int k = 0; k < b.Count; k++)
                    {
                        if (a[i] == b[k])
                        {
                            commonTerms.Add(a[i]);
                            a.RemoveAt(i);
                            b.RemoveAt(k);
                            i--;
                            break;
                        }
                    }
                }

                if (a.Count == 0) a.Add(1);
                if (b.Count == 0) a.Add(1);

                commonTerms.Add(new Addition((a.Count == 1) ? a[0] : new Multiplication(a.ToArray()), (b.Count == 1) ? b[0] : new Multiplication(b.ToArray())).Reduce());

                for (int i = 0; i < commonTerms.Count; i++)
                {
                    if (commonTerms[i].GetType() == typeof(Real) && ((Real)commonTerms[i]).Value == 0)
                    {
                        return 0;
                    }
                }

                if (commonTerms.Count == 1)
                {
                    Term term = commonTerms[0];
                    if (IsMulInverse) term.IsMulInverse = !commonTerms[0].IsMulInverse;
                    if (IsMulInverse) term.IsAddInverse = !commonTerms[0].IsAddInverse;
                    return term;
                }

                return new Multiplication(IsAddInverse, IsMulInverse, commonTerms.ToArray());
            }

            if (Parameters.Any(x => x == t))
            {
                IEnumerable<Term> terms = Parameters.Where(x => x != t);
                return new Multiplication(IsAddInverse, IsMulInverse, t, new Addition(terms.Count() > 1 ? new Multiplication(terms.ToArray()) : terms.First(), 1).Reduce());
            }

            return null;
        }
    }
}
