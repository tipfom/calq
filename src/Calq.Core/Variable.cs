﻿using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Symbolics;

namespace Calq.Core
{
    public class Variable : Term
    {
        public enum VarType
        {
            Euler, Pi, Imaginary,
            Integer, Double,
            PositiveInfinity, NegativeInfinity,
            Variable
        }

        public VarType Type;
        private static Dictionary<string, VarType> ConstantsRepresentations = new Dictionary<string, VarType>()
        {
            { "e", VarType.Euler },

            { "pi", VarType.Pi },
            { "π", VarType.Euler },

            { "i", VarType.Imaginary },
            { "I", VarType.Imaginary },

            { "oo", VarType.PositiveInfinity },
            { "pinf", VarType.PositiveInfinity },
            { "ninf", VarType.NegativeInfinity },

        };

        public readonly string Name;

        public Variable(string name)
        {
            Name = name;

            Type = VarType.Variable;
            if (ConstantsRepresentations.ContainsKey(name))
            {
                Type = ConstantsRepresentations[name];
                return;
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

        public override Term Evaluate()
        {
            return this;
        }

        public override Expression GetAsExpression()
        {
            switch (Type)
            {
                case VarType.Euler: return Expression.E;
                case VarType.Pi: return Expression.Pi;
                case VarType.Imaginary: return Expression.I;
                case VarType.PositiveInfinity: return Expression.PositiveInfinity;
                case VarType.NegativeInfinity: return Expression.NegativeInfinity;
                case VarType.Integer: return Expression.FromInt64(long.Parse(Name));
                case VarType.Double: return Expression.Real(double.Parse(Name));
                default: return Expression.Symbol(Name);
            }
        }

        public override IEnumerable<string> GetVariableNames()
        {
            yield return Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override string ToInfix()
        {
            return Name;
        }

        public override Term Differentiate(string argument)
        {
            if (Name == argument) return new Variable("1");
            else return new Variable("0");
        }
        public override string ToLaTeX()
        {
            if (Type == VarType.Imaginary) return @"i";
            return Name;
        }
    }
}
