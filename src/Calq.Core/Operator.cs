using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calq.Core
{
    public class Operator
    {
        const int NumberOfInfixOperators = 6;
        public enum Operators
        {
            //Infix
            Equals,
            Addition, Subtraktion,
            Multiplication, Division,
            Power,

            //Prefix
            Sqrt, Log, Sin, Cos,

            //Python-commands
            Lim, Int,
            Solve,

            Unknown
        }
        public enum ParseResult
        {
            succesful,
            paraCountMissmatch,
            unkownName
        }

        public readonly Operators Name;
        public readonly bool IsInfix;
        public readonly string[] StringRep;
        public readonly List<int> ParaCount;

        public Operator(Operators name, bool isInfix, string[] stringRep, List<int> paraCount)
        {
            Name = name;
            IsInfix = isInfix;
            StringRep = stringRep;
            ParaCount = paraCount;
        }

        public bool TryParse(string s, out Function f)
        {
            f = null;
            if (IsInfix)
            {
                for(int i = 0; i < StringRep.Length; i++)
                {
                    int bracketDepth = 0;
                    List<int> splits = new List<int>();
                    for (int j = 0; j < s.Length; j++)
                    {
                        if (s[j] == '(') bracketDepth++;
                        if (s[j] == ')') bracketDepth--;

                        if (s[j] == StringRep[i][0] && bracketDepth == 0)
                        {
                            if (StringRep[i][0] == '-')
                            {
                                if (j > 0)
                                    if (!StringRep.Contains(s[j - 1].ToString()))
                                        splits.Add(j);
                            }
                            else splits.Add(j);
                        }
                    }

                    if (splits.Count > 0)
                    {
                        splits.Insert(0, -1);
                        splits.Add(s.Length);

                        f = new Function(this, new List<Term>(splits.Count - 1));
                        for (int j = 1; j < splits.Count; j++)
                        {
                            f.Parameter.Add(Term.TermFromMixedString(s.Substring(splits[j - 1] + 1, splits[j] - splits[j - 1] - 1)));
                        }

                        return true;
                    }
                }
            }
            else
            {
                for(int i = 0; i < StringRep.Length; i++)
                {
                    if (s.StartsWith(StringRep[i]))
                    {
                        int bracketDepth = 0;
                        List<int> splits = new List<int>();
                        s = s.Substring(StringRep[i].Length);
                        for (int j = 0; j < s.Length - 1; j++)
                        {
                            if (s[j] == '(') bracketDepth++;
                            if (s[j] == ')') bracketDepth--;

                            if (bracketDepth == 0)
                            {
                                return false;
                            }
                            if(s[j] == ',' && bracketDepth == 1)
                            {
                                splits.Add(j - 1);
                            }
                        }
                        s = s.Substring(1, s.Length - 2);

                        if (!ParaCount.Contains(splits.Count + 1))
                            throw new InvalidParameterCountException(s);
                    

                        splits.Insert(0, -1);
                        splits.Add(s.Length);

                        f = new Function(this, new List<Term>(splits.Count - 1));
                        for (int j = 1; j < splits.Count; j++)
                        {
                            f.Parameter.Add(Term.TermFromMixedString(s.Substring(splits[j - 1] + 1, splits[j] - splits[j - 1] - 1)));
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
