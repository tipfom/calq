using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Variable : Symbol
    {
        public string Name;

        public Variable(string name) : base(SymbolType.Variable, false, false)
        {
            Name = name;
        }
        public Variable(string name, bool isAddInverse, bool isMultInverse) : base(SymbolType.Variable, isAddInverse, isMultInverse)
        {
            Name = name;
        }

        public override Term Approximate()
        {
            return this;
        }
        public override Term Evaluate()
        {
            return this;
        }

        public override Term GetDerivative(string argument)
        {
            if (Name == argument)
                return new Real(1);
            else
                return new Real(0);      
        }

        public override Term Clone()
        {
            return new Variable(Name, IsAddInverse, IsMulInverse);
        }

        public override HashSet<string> GetVariableNames()
        {
            return new HashSet<string>() { Name };
        }

        public override string ToLaTeX()
        {
            string[] coolGreekLetters = new string[]
            {
                "lambda", "Lambda", "Gamma", "gamma", "Delta", "epsilon", "eta", "theta", "my",
                "sigma", "Psi", "Phi", "phi", "Omega"
            };

            if (coolGreekLetters.Contains(Name))
            {
                return "\\" + Name;
            }
            return GetSign() + Name;
        }
        public override string ToInfix()
        {
            return GetSign() + Name;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
