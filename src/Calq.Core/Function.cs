using System.Collections.Generic;
using System.Linq;
using MathNet.Symbolics;


namespace Calq.Core
{
    public class Function : Term
    {
        private static Operator Add { get { return InfixOperators[1]; } }
        private static Operator Sub { get { return InfixOperators[2]; } }
        private static Operator Mul { get { return InfixOperators[3]; } }
        private static Operator Div { get { return InfixOperators[4]; } }
        private static Operator Pow { get { return InfixOperators[5]; } }

        private static Operator Log { get { return PrefixOperators[1]; } }

        private static readonly Operator[] InfixOperators = new Operator[]
        {
            new Operator(Operator.Operators.Equals, true, new string[]{"==", "="}, null),
            new Operator(Operator.Operators.Addition, true, new string[]{"+"}, null),
            new Operator(Operator.Operators.Subtraktion, true, new string[]{"-"}, null),
            new Operator(Operator.Operators.Multiplication, true, new string[]{"*"}, null),
            new Operator(Operator.Operators.Division, true, new string[]{"/"}, null),
            new Operator(Operator.Operators.Power, true, new string[]{"^"}, null),
        };
        private static readonly Operator[] PrefixOperators = new Operator[]
        {
            new Operator(Operator.Operators.Sqrt, false, new string[]{"sqrt"}, new List<int>(){ 1 }),
            new Operator(Operator.Operators.Log, false, new string[]{"ln"}, new List<int>(){ 1 }),
            new Operator(Operator.Operators.Log, false, new string[]{"log"}, new List<int>(){ 1, 2 }),
            new Operator(Operator.Operators.Sin, false, new string[]{"sin"}, new List<int>(){ 1 }),
            new Operator(Operator.Operators.Cos, false, new string[]{"cos"}, new List<int>(){ 1 }),
            new Operator(Operator.Operators.Differentiate, false, new string[]{"dif"}, new List<int>(){ 2 }),

            new Operator(Operator.Operators.Lim, false, new string[]{ "limes", "lim" }, new List<int>(){ 3, 4 }),
            new Operator(Operator.Operators.Int, false, new string[]{ "∫", "integral", "int", }, new List<int>(){ 2, 4 }),

            new Operator(Operator.Operators.Solve, false, new string[]{"solve"}, new List<int>(){ 2 }),
            new Operator(Operator.Operators.Erf, false, new string[]{"erf"}, new List<int>(){ 2 })
        };

        public readonly Operator Operator;
        public readonly List<Term> Parameter;

        public Function(Operator op, List<Term> parameter)
        {
            Operator = op;
            Parameter = parameter;
        }
        public Function(Operator op, params Term[] parameter)
        {
            Operator = op;
            Parameter = parameter.ToList();
        }

        public static Function FunctionFromMixedString(string s)
        {
            for (int i = 0; i < PrefixOperators.Length; i++)
            {
                if (PrefixOperators[i].TryParse(s, out Function t))
                {
                    return t;
                }
            }

            //infix-operators
            for (int i = 0; i < InfixOperators.Length; i++)
            {
                if (InfixOperators[i].TryParse(s, out Function t))
                {
                    return t;
                }
            }

            //5x or 0.5x
            const string numbers = "0123456789.";
            if (numbers.Contains(s[0]))
            {
                int counter = 1;
                while (counter < s.Length)
                {
                    if (!numbers.Contains(s[counter])) break;

                    counter++;
                }

                if (s.Length > counter)
                    return new Function(InfixOperators[3], new List<Term>(2) { new Variable(s.Substring(0, counter)), TermFromMixedString(s.Substring(counter)) });
            }

            //-sign
            if (s[0] == '-')
            {
                return new Function(InfixOperators[3], new List<Term>(2) { new Variable("-1"), TermFromMixedString(s.Substring(1)) });
            }

            if (s[0] == '(' && s[s.Length - 1] == ')')
            {
                return FunctionFromMixedString(s.Substring(1, s.Length - 2));
            }

            return null;
        }

        public override Expression Evaluate()
        {
            Expression buffer;
            switch (Operator.Name)
            {
                case Operator.Operators.Equals:
                    return Infix.Format(Parameter[0].Evaluate()) == Infix.Format(Parameter[1].Evaluate()) ? Expression.Symbol("True") : Expression.Symbol("False");

                case Operator.Operators.Int:
                    // TODO:
                    // - bessere Unterstützung der Python Formattierung (** ist dort ^, pinf ist oo, ninf ist -oo)
                    // - unbekannte Funktionen zulassen oder alle von Python (inverse Gaus usw) hinzufügen, oder beides am besten
                    if (Parameter.Count == 2)
                    {
                        string pExpr;
                        WebHelper.GetIntegral(Parameter[0].ToString(), GetVariableNames().Distinct(), Parameter[1].ToString(), out pExpr);
                        return TermFromMixedString(pExpr.Replace("**", "^")).Evaluate();
                    }
                    else if (Parameter.Count == 4)
                    {

                        string pExpr;
                        WebHelper.GetIntegral(Parameter[0].ToString(), GetVariableNames().Distinct(), Parameter[1].ToString(), Parameter[2].ToString(), Parameter[3].ToString(), out pExpr);
                        return TermFromMixedString(pExpr.Replace("**", "^")).Evaluate();
                    }
                    return null;
                case Operator.Operators.Lim:
                    if (Parameter.Count == 3)
                    {
                        string pExpr;
                        WebHelper.GetLimit(Parameter[0].ToString(), GetVariableNames().Distinct(), Parameter[1].ToString(), Parameter[2].ToString(), out pExpr);
                        return TermFromMixedString(pExpr.Replace("**", "^")).Evaluate();
                    }
                    else if (Parameter.Count == 4)
                    {

                        string pExpr;
                        WebHelper.GetLimit(Parameter[0].ToString(), GetVariableNames().Distinct(), Parameter[1].ToString(), Parameter[2].ToString(), Parameter[3].ToString(), out pExpr);
                        return TermFromMixedString(pExpr.Replace("**", "^")).Evaluate();
                    }
                    return null;
                case Operator.Operators.Solve:
                    if (Parameter.Count == 2)
                    {
                        List<string> stringExpressions = new List<string>();
                        List<string> variables = new List<string>();
                        List<string> solveFors = new List<string>();
                        if (Parameter[0].GetType() == typeof(TermList))
                        {
                            TermList termList = Parameter[0] as TermList;
                            foreach (Term t in termList.Terms)
                            {
                                stringExpressions.Add(t.ToString());
                                variables.AddRange(t.GetVariableNames());
                            }
                        }
                        else
                        {
                            stringExpressions.Add(Parameter[0].ToString());
                            variables.AddRange(Parameter[0].GetVariableNames());
                        }

                        if (Parameter[1].GetType() == typeof(TermList))
                        {
                            TermList termList = Parameter[1] as TermList;
                            foreach (Term t in termList.Terms)
                            {
                                solveFors.Add(t.ToString());
                            }
                        }
                        else
                        {
                            solveFors.Add(Parameter[1].ToString());
                        }


                        string pExpr;
                        WebHelper.GetSolve(stringExpressions, variables.Distinct(), solveFors, out pExpr);
                        System.Diagnostics.Debug.Print(pExpr);
                    }
                    return null;

                case Operator.Operators.Addition:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer += Parameter[i].Evaluate();
                    return buffer;
                case Operator.Operators.Subtraktion:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer -= Parameter[i].Evaluate();
                    return buffer;
                case Operator.Operators.Multiplication:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer *= Parameter[i].Evaluate();
                    return buffer;
                case Operator.Operators.Division:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer /= Parameter[i].Evaluate();
                    return buffer;
                case Operator.Operators.Power:
                    buffer = Parameter[0].Evaluate();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer = Expression.Pow(buffer, Parameter[i].Evaluate());
                    return buffer;

                case Operator.Operators.Sqrt:
                    return Expression.Sqrt(Parameter[0].Evaluate());
                case Operator.Operators.Log:
                    if (Parameter.Count == 1) return Expression.Ln(Parameter[0].Evaluate());
                    else return Expression.Log(Parameter[0].Evaluate(), Parameter[1].Evaluate());
                case Operator.Operators.Sin:
                    return Expression.Sin(Parameter[0].Evaluate());
                case Operator.Operators.Cos:
                    return Expression.Cos(Parameter[0].Evaluate());
            }

            return null;
        }

        public override Expression GetAsExpression()
        {
            Expression buffer;
            switch (Operator.Name)
            {
                case Operator.Operators.Equals:
                    return Infix.Format(Parameter[0].Evaluate()) == Infix.Format(Parameter[1].Evaluate()) ? Expression.Symbol("True") : Expression.Symbol("False");

                case Operator.Operators.Int:
                    return Expression.Symbol(ToInfix());
                case Operator.Operators.Lim:
                    return Expression.Symbol(ToInfix());
                case Operator.Operators.Solve:
                    return Expression.Symbol(ToInfix());

                case Operator.Operators.Differentiate:
                    return Parameter[1].Differentiate(Parameter[0].ToString()).GetAsExpression();
                case Operator.Operators.Addition:
                    buffer = Parameter[0].GetAsExpression();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer += Parameter[i].GetAsExpression();
                    return buffer;
                case Operator.Operators.Subtraktion:
                    buffer = Parameter[0].GetAsExpression();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer -= Parameter[i].GetAsExpression();
                    return buffer;
                case Operator.Operators.Multiplication:
                    buffer = Parameter[0].GetAsExpression();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer *= Parameter[i].GetAsExpression();
                    return buffer;
                case Operator.Operators.Division:
                    buffer = Parameter[0].GetAsExpression();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer /= Parameter[i].GetAsExpression();
                    return buffer;
                case Operator.Operators.Power:
                    buffer = Parameter[0].GetAsExpression();
                    for (int i = 1; i < Parameter.Count; i++)
                        buffer = Expression.Pow(buffer, Parameter[i].GetAsExpression());
                    return buffer;

                case Operator.Operators.Sqrt:
                    return Expression.Sqrt(Parameter[0].GetAsExpression());
                case Operator.Operators.Log:
                    if (Parameter.Count == 1) return Expression.Ln(Parameter[0].GetAsExpression());
                    else return Expression.Log(Parameter[0].GetAsExpression(), Parameter[1].GetAsExpression());
                case Operator.Operators.Sin:
                    return Expression.Sin(Parameter[0].GetAsExpression());
                case Operator.Operators.Cos:
                    return Expression.Cos(Parameter[0].GetAsExpression());
            }

            return null;
        }

        public override string ToInfix()
        {
            if (Operator.IsInfix)
            {
                return string.Join(Operator.StringRep[0], Parameter.Select(x => x.ToInfix()));
            }
            else
            {
                return Operator.StringRep[0] + "(" + string.Join(",", Parameter.Select(x => x.ToInfix())) + ")";
            }
        }

        public override IEnumerable<string> GetVariableNames()
        {
            foreach (Term param in Parameter)
            {
                foreach (string variable in param.GetVariableNames())
                    yield return variable;
            }
        }

        public override Term Differentiate(string argument)
        {
            Function ret;

            switch (Operator.Name)
            {
                case Operator.Operators.Addition:
                    return new Function(Operator, Parameter.Select(x => x.Differentiate(argument)).ToList());
                case Operator.Operators.Subtraktion:
                    return new Function(Operator, Parameter.Select(x => x.Differentiate(argument)).ToList());

                case Operator.Operators.Multiplication:
                    ret = new Function(Add, new List<Term>());
                    for (int i = 0; i < Parameter.Count; i++)
                    {
                        Function f = new Function(Mul, new List<Term>());
                        for (int j = 0; j < Parameter.Count; j++)
                        {
                            if (i == j) f.Parameter.Add(Parameter[j].Differentiate(argument));
                            else f.Parameter.Add(Parameter[j]);
                        }

                        ret.Parameter.Add(f);
                    }
                    return ret;
                case Operator.Operators.Division:
                    Term g = new Function(Mul, new List<Term>());

                    if (Parameter.Count == 2)
                    {
                        g = Parameter[1];
                    }
                    else
                    {
                        g = new Function(Mul, new List<Term>());

                        for (int i = 1; 1 < Parameter.Count; i++)
                        {
                            ((Function)g).Parameter.Add(Parameter[i]);
                        }
                    }

                    ret = new Function(Div, new List<Term>());
                    ret.Parameter.Add(new Function(Sub, new List<Term>(2)
                    {
                        new Function(Mul, new List<Term>(2) { Parameter[0].Differentiate(argument), g}),
                        new Function(Mul, new List<Term>(2) { Parameter[0], g.Differentiate(argument)})
                    }));
                    ret.Parameter.Add(new Function(Pow, new List<Term>(2)
                    {
                        Parameter[1],
                        new Variable("2")
                    }));

                    return ret;
                case Operator.Operators.Power:
                    Term t = Parameter[0];
                    g = new Function(Mul, new List<Term>());
                    if (Parameter.Count == 2)
                    {
                        g = Parameter[1];
                    }
                    else
                    {
                        g = new Function(Pow, new List<Term>());

                        for (int i = 1; 1 < Parameter.Count; i++)
                        {
                            ((Function)g).Parameter.Add(Parameter[i]);
                        }
                    }

                    ret = new Function(Mul, new List<Term>());
                    ret.Parameter.Add(new Function(Pow, t, new Function(Sub, g, new Variable("1"))));
                    ret.Parameter.Add(new Function(Add,
                        new Function(Mul, g, t.Differentiate(argument)),
                        new Function(Mul, t, new Function(Log, t), g.Differentiate(argument))));

                    return ret;
            }

            return null;
        }

        //Returns prefixString
        public override string ToString()
        {
            return Operator.StringRep[0] + "[" + string.Join(",", Parameter) + "]";
        }

        public override string ToLaTeX()
        {
            switch (Operator.Name)
            {
                case Operator.Operators.Equals:
                    //return Infix.Format(Parameter[0].Evaluate()) == Infix.Format(Parameter[1].Evaluate()) ? Expression.Symbol("True") : Expression.Symbol("False");
                    return null;

                case Operator.Operators.Int:
                    // TODO:
                    // - bessere Unterstützung der Python Formattierung (** ist dort ^, pinf ist oo, ninf ist -oo)
                    // - unbekannte Funktionen zulassen oder alle von Python (inverse Gaus usw) hinzufügen, oder beides am besten
                    if (Parameter.Count == 2)
                    {
                        return $@"\int {Parameter[0].ToLaTeX()} d{Parameter[1].ToLaTeX()}";
                    }
                    else if (Parameter.Count == 4)
                    {
                        return $@"\int_{"{" + Parameter[2].ToLaTeX() + "}"}^{"{" + Parameter[3].ToLaTeX() + "}"} {Parameter[0].ToLaTeX()} d{Parameter[1].ToLaTeX()}";
                    }
                    return null;
                case Operator.Operators.Lim:
                    if (Parameter.Count == 3)
                    {
                        return $@"\lim_{"{" + Parameter[1].ToLaTeX()}\to{Parameter[2].ToLaTeX() + "}"} {Parameter[0].ToLaTeX()}";
                    }
                    else if (Parameter.Count == 4)
                    {
                        // TODO : +-
                        string dirLatex = Parameter[3].ToLaTeX() == "l" ? "^{-}" : ((Parameter[3].ToLaTeX() == "r") ? "^{+}" : "");
                        return $@"\lim_{"{" + Parameter[1].ToLaTeX()} \to {Parameter[2].ToLaTeX() + dirLatex + "}"} {Parameter[0].ToLaTeX()}";
                    }
                    return null;
                case Operator.Operators.Solve:

                    return null;

                case Operator.Operators.Addition:
                    string res = Parameter[0].ToLaTeX();
                    for (int i = 1; i < Parameter.Count; i++)
                        res += "+" + Parameter[i].ToLaTeX();
                    return res;
                case Operator.Operators.Subtraktion:
                    res = Parameter[0].ToLaTeX();
                    for (int i = 1; i < Parameter.Count; i++)
                        res += "-" + Parameter[i].ToLaTeX();
                    return res;
                case Operator.Operators.Multiplication:
                    if (Parameter.Count == 2)
                    {
                        if (Parameter[0].ToInfix() == "-1")
                        {
                            return "-" + Parameter[1].ToLaTeX();
                        }
                        if (Parameter[1].ToInfix() == "-1")
                        {
                            return "-" + Parameter[0].ToLaTeX();
                        }
                    }
                    res = Parameter[0].ToLaTeX();
                    for (int i = 1; i < Parameter.Count; i++)
                        res += "\\cdot " + Parameter[i].ToLaTeX();
                    return res;
                case Operator.Operators.Division:
                    return $@"\frac{"{" + Parameter[0].ToLaTeX() + "}"}{"{" + Parameter[1].ToLaTeX() + "}"}";
                case Operator.Operators.Power:
                    return $@"{Parameter[0].ToLaTeX()}^{"{" + Parameter[1].ToLaTeX() + "}"}";

                case Operator.Operators.Sqrt:
                    return $@"\sqrt{"{" + Parameter[0].ToLaTeX() + "}"}";
                case Operator.Operators.Log:
                    if (Parameter.Count == 1) return $@"\log{"{" + Parameter[0].ToLaTeX() + "}"}";
                    return $@"\log_{"{" + Parameter[1].ToLaTeX() + "}"} {Parameter[0].ToLaTeX()}";
                case Operator.Operators.Sin:
                    return $@"\sin ({Parameter[0].ToLaTeX()})";
                case Operator.Operators.Cos:
                    return $@"\cos ({Parameter[0].ToLaTeX()})";
            }

            return null;
        }
    }
}
