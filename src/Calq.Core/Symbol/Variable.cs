using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Variable : Symbol
    {
        public string Name;

        public Variable(string name) : base(SymbolType.Variable)
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

        public override Term Differentiate(string argument)
        {
            if (Name == argument)
                return new Real(1);
            else
                return new Real(0);      
        }

        public override HashSet<string> GetVariableNames()
        {
            return new HashSet<string>() { Name };
        }

        //[TODO] Latexcode phi, psi ...
        public override string ToLaTeX()
        {
            return Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
