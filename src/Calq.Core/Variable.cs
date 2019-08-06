using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class Variable : Term
    {
        public enum VarType
        {
            E, Pi, Phi,
            Variable
        }

        public VarType Type;
        private static string[] ConstantsRep = new string[]
        {
            "e", "π", "φ"
        };

        public readonly string Name;

        public Variable(string name)
        {
            Name = name;
            
            for(int i = 0; i < ConstantsRep.Length; i++)
            {
                if(Name == ConstantsRep[i])
                {
                    Type = (VarType)i;
                    break;
                }
            }
        }

        public override Term Approximate()
        {
            throw new NotImplementedException();
        }

        public override Term Evaluate()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
