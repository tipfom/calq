using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Logarithm : Function
    {
        public Logarithm(params Term[] p) : base(FuncType.Log, false, false, p)
        {
            if (!(p.Length == 1 || p.Length == 2))
                throw new InvalidParameterCountException("Log takes one or two arguments");
        }
        public Logarithm(bool isAddInverse, bool isMulInverse, params Term[] p) : base(FuncType.Log, isAddInverse, isMulInverse, p)
        {
            if (!(p.Length == 1 || p.Length == 2))
                throw new InvalidParameterCountException("Log takes one or two arguments");
        }

        public override Term Approximate()
        {
            throw new NotImplementedException();
        }

        public override Term GetDerivative(string argument)
        {
            if (Parameters.Length == 1)
            {
                return Parameters[0].GetDerivative(argument) / Parameters[0];
            }
            else
            {
                return (new Logarithm(Parameters[0]) / new Logarithm(Parameters[1])).GetDerivative(argument);
            }
        }

        public override Term Evaluate()
        {
            return this;
        }

        public override string ToLaTeX()
        {
            if (Parameters.Length == 1)
                return GetSign() + $@"\log{"(" + Parameters[0].ToLaTeX() + ")"}";
            else
                return GetSign() + $@"\log_{"{" + Parameters[1].ToLaTeX() + "}"} {"(" + Parameters[0].ToLaTeX() + ")"}";
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Term Reduce()
        {
            // cases to handle:
            // 1.) inner is power
            // 2.) inner is equal to base
            // 3.) inner is multiplication or division ? (this should probably not be reduced)

            Term inner = Parameters[0].Reduce();
            if (inner.GetType() == typeof(Power))
            {
                Function finner = inner as Function;
                if (Parameters.Length > 1)
                    return (finner.Parameters[1] * new Logarithm(IsAddInverse, IsMulInverse, finner.Parameters[0], Parameters[1].Reduce())).Reduce();
                else
                    return (finner.Parameters[1] * new Logarithm(IsAddInverse, IsMulInverse, finner.Parameters[0])).Reduce();
            }

            if (inner.Type == TermType.Symbol)
            {
                if (Parameters.Length == 1)
                {
                    if (Parameters[0].GetType() == typeof(Constant) && ((Constant)Parameters[0]).Name == Constant.ConstType.E) return 1;
                }
                else
                {
                    if (Parameters[1].Reduce() == inner) return 1;
                }
            }

            if (Parameters.Length > 1)
                return new Logarithm(IsAddInverse, IsMulInverse, inner, Parameters[1].Reduce());
            else
                return new Logarithm(IsAddInverse, IsMulInverse, inner);
        }

        public override Term CheckAddReduce(Term t)
        {
            if (t.GetType() == typeof(Logarithm))
            {
                Logarithm tAsLogarithm = t as Logarithm;
                if(Parameters.Length == 1 && tAsLogarithm.Parameters.Length == 1)
                {
                    return new Logarithm((Parameters[0] * tAsLogarithm.Parameters[0]).Reduce());
                }
                else if(Parameters.Length == 2 && tAsLogarithm.Parameters.Length == 2 && Parameters[1] == tAsLogarithm.Parameters[1])
                {
                    return new Logarithm((Parameters[0] * tAsLogarithm.Parameters[0]).Reduce(), Parameters[1]);
                }
            }
            return null;
        }
    }
}
