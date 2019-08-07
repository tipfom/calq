using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Symbolics;

namespace Calq.Core
{
    public class Variable : Term
    {
        public enum VarType
        {
            E, Pi, I,
            Integer, Double,
            Variable
        }

        public VarType Type;
        private static string[] ConstantsRep = new string[]
        {
            "e", "π", "i"
        };

        public readonly string Name;

        public Variable(string name)
        {
            Name = name;

            Type = VarType.Variable;
            for(int i = 0; i < ConstantsRep.Length; i++)
            {
                if (Name == ConstantsRep[i])
                {
                    Type = (VarType)i;
                    return;
                }
            }

            const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

            for(int i = 0; i < Alphabet.Length; i++)
            {
                if (name.Contains(Alphabet[i].ToString()))
                {
                    return;
                }
            }

            if (name.Contains("."))
                Type = VarType.Double;
            else
                Type = VarType.Integer;
            
        }

        public override Expression Evaluate()
        {
            switch (Type)
            {
                case VarType.E: return Expression.E;
                case VarType.Pi: return Expression.Pi;
                case VarType.I: return Expression.I;
                case VarType.Integer: return Expression.FromInt64(long.Parse(Name));
                case VarType.Double: return Expression.Real(double.Parse(Name));
                default: return Expression.Symbol(Name);
            }
        }

        public override Expression GetAsExpression()
        {
            return Evaluate();
        }

        public override IEnumerable<string> GetVariableNames()
        {
            yield return Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override string GetInfix()
        {
            return Name;
        }

    }
}
