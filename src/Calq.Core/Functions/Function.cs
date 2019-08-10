﻿using System.Collections.Generic;

namespace Calq.Core
{
    public abstract class Function : Term
    {
        public enum FuncType
        {
            Equals,
            Addition,
            Multiplication,
            Power,

            Sqrt, Log, Sin, Cos, Differentiate,

            Lim, Int,
            Solve, Erf,

            Unknown
        }

        public readonly FuncType Name;
        public readonly Term[] Parameters;
        protected Function(FuncType name, Term[] paras) : base(TermType.Function)
        {
            Name = name;
            Parameters = paras;
        }

        public static Function PrefixFuncFromString(string s)
        {
            string fName = "";
            if (s.Contains("("))
                fName = s.Split('(')[0];           
            else
                return null;
            
            FuncType type = FuncType.Unknown;
            switch (fName)
            {
                case "sin": type = FuncType.Sin; break;
                case "cos": type = FuncType.Cos; break;
                default: return null;
            }

            s = s.Substring(fName.Length);
            s.Substring(1, s.Length - 2);

            List<int> splits = new List<int>(6);
            int bracketDepth = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(' || s[i] == '{') bracketDepth++;
                if (s[i] == ')' || s[i] == '}') bracketDepth--;

                if (s[i] == ',' && bracketDepth == 0)
                {
                    splits.Add(i);
                }
            }

            Term[] paras = new Term[splits.Count + 1];

            splits.Insert(0, -1);
            splits.Add(s.Length);

            for (int j = 1; j < splits.Count; j++)
            {
                paras[j - 1] = Parse(s.Substring(splits[j - 1] + 1, splits[j] - splits[j - 1] - 1));
            }

            switch (type)
            {
                case FuncType.Sin: return new Sin(paras);
                case FuncType.Cos: return new Cos(paras);
            }

            return null;
        }
        public static Function InfixFuncFromString(string s)
        {
            string name = "";
            int smalestOrder = int.MaxValue;
            int charPos = -1;
            int bracketDepth = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(' || s[i] == '{') bracketDepth++;
                if (s[i] == ')' || s[i] == '}') bracketDepth--;

                if (bracketDepth == 0)
                {
                    int order = Function.GetOrder(s[i].ToString());
                    if (order < smalestOrder)
                    {
                        smalestOrder = order;
                        charPos = i;
                        name = s[i].ToString();
                    }
                }
            }

            if (smalestOrder == int.MaxValue)
                return null;

            switch (name)
            {
                case "+": return Parse(s.Substring(0, charPos)) + Parse(s.Substring(charPos + 1));
                case "-":
                    if (charPos == 0)
                        return -Parse(s.Substring(1));
                    else
                        return Parse(s.Substring(0, charPos)) - Parse(s.Substring(charPos + 1));
                case "*": return Parse(s.Substring(0, charPos)) * Parse(s.Substring(charPos + 1));
                case "/": return Parse(s.Substring(0, charPos)) / Parse(s.Substring(charPos + 1));
                case "^": return Parse(s.Substring(0, charPos)) ^ Parse(s.Substring(charPos + 1));
            }

            throw new System.FormatException();
        }
        //[TODO] Reihenfolge checken
        public static bool operator ==(Function a, Function b)
        {
            if (a is null || a is null)
                return a is null && a is null;

            if (a.Name != b.Name) return false;
            if (a.Parameters.Length != b.Parameters.Length) return false;

            for (int i = 0; i < a.Parameters.Length; i++)
            {
                if (a.Parameters[i] != b.Parameters[i]) return false;
            }

            return true;
        }
        public static bool operator !=(Function a, Function b)
        {
            return !(a == b);
        }

        public override HashSet<string> GetVariableNames()
        {
            HashSet<string> ret = new HashSet<string>();

            foreach (Term t in Parameters)
                ret.UnionWith(t.GetVariableNames());

            return ret;
        }
 
        public static FuncType InfixOperator(string name)
        {
            switch (name)
            {
                case "=": return FuncType.Equals;
                case "+": return FuncType.Addition;
                case "-": return FuncType.Addition;
                case "*": return FuncType.Multiplication;
                case "/": return FuncType.Multiplication;
                case "^": return FuncType.Power;
            }

            return FuncType.Unknown;
        }
        public static bool IsInverseOperator(string name)
        {
            return name == "-" || name == "/";
        }
        public static int GetOrder(FuncType func)
        {
            switch (func)
            {
                case FuncType.Equals: return 0;
                case FuncType.Addition: return 1;
                case FuncType.Multiplication: return 2;
                case FuncType.Power: return 2;
                default: return int.MaxValue;
            }
        }
        public static int GetOrder(string name)
        {
            return GetOrder(InfixOperator(name));
        }
    }
}
